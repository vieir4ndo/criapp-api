using System.Linq.Expressions;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Resources;
using CRIApp.Application.Services;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace CRIApp.Application.Tests.Services;

public class QuestionServiceTests
{
    private readonly Mock<IQuestionRepository> _questionRepositoryMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IStringLocalizer<AppMessages>> _localizerMock;
    private readonly QuestionService _sut;

    public QuestionServiceTests()
    {
        _questionRepositoryMock = new Mock<IQuestionRepository>();
        _userServiceMock = new Mock<IUserService>();
        _localizerMock = new Mock<IStringLocalizer<AppMessages>>();
        _sut = new QuestionService(_questionRepositoryMock.Object, _userServiceMock.Object, NullLogger<QuestionService>.Instance, _localizerMock.Object);
    }

    private static Question BuildQuestion(QuestionGroup group = QuestionGroup.CognitiveDevelopment)
        => new Question("Is your child eating well?", Guid.NewGuid(), QuestionType.Agreement, group: group);

    private static User BuildUser(FamilyType? familyType = FamilyType.HeterosexualCouple)
        => new User("Parent", new DateTime(1985, 1, 1), Sex.Male, "parent@domain.com", "pass123", familyType);

    private static User BuildUserWithAnswers(Guid childId, int answersToday, int answersOtherChild = 0, FamilyType? familyType = FamilyType.HeterosexualCouple)
    {
        var user = new User("Parent", new DateTime(1985, 1, 1), Sex.Male, "parent@domain.com", "pass123", familyType);

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

    private void SetupGetFiltered(List<Question> preferred, List<Question>? fallback = null)
        => _questionRepositoryMock
            .SetupSequence(r => r.GetFiltered(It.IsAny<Expression<Func<Question, bool>>>()))
            .ReturnsAsync(preferred)
            .ReturnsAsync(fallback ?? new List<Question>());

    [Fact]
    public async Task GetQuestionsToReply_NoAnswersToday_ShouldReturnUpTo5()
    {
        var userId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var user = BuildUser();
        var questions = Enumerable.Range(0, 15).Select(_ => BuildQuestion()).ToList();

        _userServiceMock.Setup(s => s.GetAndValidateById(userId)).ReturnsAsync(user);
        SetupGetFiltered(questions);

        var result = await _sut.GetQuestionsToReply(userId, childId);

        Assert.Equal(5, result.Count());
    }

    [Fact]
    public async Task GetQuestionsToReply_SomeAnsweredToday_ShouldReturnRemaining()
    {
        var userId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var user = BuildUserWithAnswers(childId, answersToday: 3);
        var questions = Enumerable.Range(0, 10).Select(_ => BuildQuestion()).ToList();

        _userServiceMock.Setup(s => s.GetAndValidateById(userId)).ReturnsAsync(user);
        SetupGetFiltered(questions);

        var result = await _sut.GetQuestionsToReply(userId, childId);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetQuestionsToReply_AllAnsweredToday_ShouldReturnEmpty()
    {
        var userId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var user = BuildUserWithAnswers(childId, answersToday: 10);

        _userServiceMock.Setup(s => s.GetAndValidateById(userId)).ReturnsAsync(user);

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
        SetupGetFiltered(questions);

        var result = await _sut.GetQuestionsToReply(userId, childId);

        Assert.Equal(5, result.Count());
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
    public async Task GetQuestionsToReply_ShouldReturnQuestionDtos()
    {
        var userId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var user = BuildUser();
        var question = BuildQuestion();

        _userServiceMock.Setup(s => s.GetAndValidateById(userId)).ReturnsAsync(user);
        SetupGetFiltered(new List<Question> { question });

        var result = await _sut.GetQuestionsToReply(userId, childId);

        var dto = result.Single();
        Assert.Equal(question.Id, dto.Id);
        Assert.Equal(question.Text, dto.Text);
        Assert.Equal(question.Type, dto.Type);
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

    [Fact]
    public async Task GetQuestionsToReply_UserWithoutFamilyType_ShouldReturnQuestionsWithoutFamilyFilter()
    {
        var userId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var user = BuildUser(familyType: null);
        var questions = Enumerable.Range(0, 10).Select(_ => BuildQuestion()).ToList();

        _userServiceMock.Setup(s => s.GetAndValidateById(userId)).ReturnsAsync(user);
        SetupGetFiltered(questions);

        var result = await _sut.GetQuestionsToReply(userId, childId);

        Assert.Equal(5, result.Count());
    }

    [Fact]
    public async Task GetQuestionsToReply_UserWithoutFamilyType_NoQuestions_ShouldReturnEmpty()
    {
        var userId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var user = BuildUser(familyType: null);

        _userServiceMock.Setup(s => s.GetAndValidateById(userId)).ReturnsAsync(user);
        SetupGetFiltered(new List<Question>());

        var result = await _sut.GetQuestionsToReply(userId, childId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetQuestionsToReply_MultipleGroups_ShouldIncludeAtLeastOnePerGroup()
    {
        var userId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var user = BuildUser();

        var preferred = new List<Question>
        {
            BuildQuestion(QuestionGroup.CognitiveDevelopment),
            BuildQuestion(QuestionGroup.CognitiveDevelopment),
            BuildQuestion(QuestionGroup.EmotionalBonding),
            BuildQuestion(QuestionGroup.EmotionalBonding),
            BuildQuestion(QuestionGroup.DailyRoutines),
            BuildQuestion(QuestionGroup.DailyRoutines),
            BuildQuestion(QuestionGroup.SocialInteraction),
            BuildQuestion(QuestionGroup.SocialInteraction),
        };

        _userServiceMock.Setup(s => s.GetAndValidateById(userId)).ReturnsAsync(user);
        SetupGetFiltered(preferred);

        var result = (await _sut.GetQuestionsToReply(userId, childId)).ToList();
        var groups = result.Select(q => q.Group).Distinct().ToList();

        Assert.Equal(5, result.Count);
        Assert.Contains(QuestionGroup.CognitiveDevelopment, groups);
        Assert.Contains(QuestionGroup.EmotionalBonding, groups);
        Assert.Contains(QuestionGroup.DailyRoutines, groups);
        Assert.Contains(QuestionGroup.SocialInteraction, groups);
    }

    [Fact]
    public async Task GetQuestionsToReply_FourGroupsWithOneQuestionEach_ShouldReturnFourQuestions()
    {
        var userId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var user = BuildUser();

        var preferred = new List<Question>
        {
            BuildQuestion(QuestionGroup.CognitiveDevelopment),
            BuildQuestion(QuestionGroup.EmotionalBonding),
            BuildQuestion(QuestionGroup.DailyRoutines),
            BuildQuestion(QuestionGroup.SocialInteraction),
        };

        _userServiceMock.Setup(s => s.GetAndValidateById(userId)).ReturnsAsync(user);
        SetupGetFiltered(preferred);

        var result = (await _sut.GetQuestionsToReply(userId, childId)).ToList();
        var groups = result.Select(q => q.Group).Distinct().ToList();

        Assert.Equal(4, result.Count);
        Assert.Equal(4, groups.Count);
    }

    [Fact]
    public async Task GetQuestionsToReply_GroupOnlyInFallback_ShouldIncludeThatGroup()
    {
        var userId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var user = BuildUser();

        var preferred = new List<Question>
        {
            BuildQuestion(QuestionGroup.CognitiveDevelopment),
            BuildQuestion(QuestionGroup.EmotionalBonding),
            BuildQuestion(QuestionGroup.DailyRoutines),
        };

        var fallback = new List<Question>
        {
            BuildQuestion(QuestionGroup.SocialInteraction),
        };

        _userServiceMock.Setup(s => s.GetAndValidateById(userId)).ReturnsAsync(user);
        SetupGetFiltered(preferred, fallback);

        var result = (await _sut.GetQuestionsToReply(userId, childId)).ToList();
        var groups = result.Select(q => q.Group).ToList();

        Assert.Contains(QuestionGroup.SocialInteraction, groups);
    }

    [Fact]
    public async Task GetQuestionsToReply_ShouldIncludeGroupFieldInDto()
    {
        var userId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var user = BuildUser();
        var question = BuildQuestion(QuestionGroup.EmotionalBonding);

        _userServiceMock.Setup(s => s.GetAndValidateById(userId)).ReturnsAsync(user);
        SetupGetFiltered(new List<Question> { question });

        var result = await _sut.GetQuestionsToReply(userId, childId);

        Assert.Equal(QuestionGroup.EmotionalBonding, result.Single().Group);
    }
}

