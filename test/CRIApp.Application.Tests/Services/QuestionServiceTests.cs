using CRIApp.Application.Abstractions;
using CRIApp.Application.Services;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Moq;
using Xunit;

namespace CRIApp.Application.Tests.Services;

public class QuestionServiceTests
{
    private readonly Mock<IQuestionRepository> _questionRepositoryMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly QuestionService _sut;

    public QuestionServiceTests()
    {
        _questionRepositoryMock = new Mock<IQuestionRepository>();
        _userServiceMock = new Mock<IUserService>();
        _sut = new QuestionService(_questionRepositoryMock.Object, _userServiceMock.Object);
    }

    private static Question BuildQuestion()
        => new Question("Is your child eating well?", Guid.NewGuid(), QuestionType.Agreement);

    private static User BuildUserWithAnswers(Guid childId, int answersToday, int answersOtherChild = 0)
    {
        var user = new User("Parent", new DateTime(1985, 1, 1), Sex.Male, "parent@domain.com", "pass123");

        var questionIdForToday = Guid.NewGuid();
        for (var i = 0; i < answersToday; i++)
        {
            var answer = new Answer(3, Guid.NewGuid(), childId, user.Id);
            user.Answers.Add(answer);
        }

        for (var i = 0; i < answersOtherChild; i++)
        {
            var answer = new Answer(2, Guid.NewGuid(), Guid.NewGuid(), user.Id);
            user.Answers.Add(answer);
        }

        return user;
    }

    [Fact]
    public async Task GetQuestionsToReply_NoAnswersToday_ShouldReturnUpTo10()
    {
        var userId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var user = new User("Parent", new DateTime(1985, 1, 1), Sex.Male, "parent@domain.com", "pass123");
        var questions = Enumerable.Range(0, 15).Select(_ => BuildQuestion()).ToList();

        _userServiceMock.Setup(s => s.GetAndValidateById(userId)).ReturnsAsync(user);
        _questionRepositoryMock.Setup(r => r.GetAllExcept(It.IsAny<List<Guid>>()))
            .ReturnsAsync(questions);

        var result = await _sut.GetQuestionsToReply(userId, childId);

        Assert.Equal(10, result.Count());
    }

    [Fact]
    public async Task GetQuestionsToReply_SomeAnsweredToday_ShouldReturnRemaining()
    {
        var userId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var user = BuildUserWithAnswers(childId, answersToday: 7);

        var questions = Enumerable.Range(0, 10).Select(_ => BuildQuestion()).ToList();

        _userServiceMock.Setup(s => s.GetAndValidateById(userId)).ReturnsAsync(user);
        _questionRepositoryMock.Setup(r => r.GetAllExcept(It.IsAny<List<Guid>>()))
            .ReturnsAsync(questions);

        var result = await _sut.GetQuestionsToReply(userId, childId);

        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetQuestionsToReply_AllAnsweredToday_ShouldReturnEmpty()
    {
        var userId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var user = BuildUserWithAnswers(childId, answersToday: 10);

        _userServiceMock.Setup(s => s.GetAndValidateById(userId)).ReturnsAsync(user);
        _questionRepositoryMock.Setup(r => r.GetAllExcept(It.IsAny<List<Guid>>()))
            .ReturnsAsync(new List<Question>());

        var result = await _sut.GetQuestionsToReply(userId, childId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetQuestionsToReply_AnswersForAnotherChild_ShouldNotAffectLimit()
    {
        var userId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var user = BuildUserWithAnswers(childId, answersToday: 0, answersOtherChild: 10);

        var questions = Enumerable.Range(0, 15).Select(_ => BuildQuestion()).ToList();

        _userServiceMock.Setup(s => s.GetAndValidateById(userId)).ReturnsAsync(user);
        _questionRepositoryMock.Setup(r => r.GetAllExcept(It.IsAny<List<Guid>>()))
            .ReturnsAsync(questions);

        var result = await _sut.GetQuestionsToReply(userId, childId);

        Assert.Equal(10, result.Count());
    }

    [Fact]
    public async Task GetQuestionsToReply_UserNotFound_ShouldThrowNotFoundException()
    {
        var userId = Guid.NewGuid();
        _userServiceMock.Setup(s => s.GetAndValidateById(userId))
            .ThrowsAsync(new NotFoundException("Usuario no encontrado"));

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.GetQuestionsToReply(userId, Guid.NewGuid()));
    }

    [Fact]
    public async Task GetAndValidateById_QuestionExists_ShouldReturnQuestion()
    {
        var questionId = Guid.NewGuid();
        var question = BuildQuestion();
        _questionRepositoryMock.Setup(r => r.GetById(questionId)).ReturnsAsync(question);

        var result = await _sut.GetAndValidateById(questionId);

        Assert.Equal(question, result);
    }

    [Fact]
    public async Task GetAndValidateById_QuestionNotFound_ShouldThrowNotFoundException()
    {
        var questionId = Guid.NewGuid();
        _questionRepositoryMock.Setup(r => r.GetById(questionId)).ReturnsAsync((Question?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetAndValidateById(questionId));
    }
}
