using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Application.Services;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace CRIApp.Application.Tests.Services;

public class AnswerServiceTests
{
    private readonly Mock<IAnswerRepository> _repositoryMock;
    private readonly Mock<IQuestionService> _questionServiceMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly AnswerService _sut;

    public AnswerServiceTests()
    {
        _repositoryMock = new Mock<IAnswerRepository>();
        _questionServiceMock = new Mock<IQuestionService>();
        _userServiceMock = new Mock<IUserService>();
        _sut = new AnswerService(_repositoryMock.Object, _questionServiceMock.Object, _userServiceMock.Object, NullLogger<AnswerService>.Instance);
    }

    private static Question BuildQuestion()
        => new Question("Does your child sleep well?", Guid.NewGuid(), QuestionType.Agreement);

    private static User BuildUserWithChild(out Guid childId)
    {
        var user = new User("Parent", new DateTime(1985, 1, 1), Sex.Male, "parent@domain.com", "pass123");
        var child = new Child("Alice", new DateTime(2020, 1, 1), Sex.Female, 6, user.Id, false);
        user.Children.Add(child);
        childId = child.Id;
        return user;
    }

    [Fact]
    public async Task Create_ValidDto_ShouldCreateAnswer()
    {
        var user = BuildUserWithChild(out var childId);
        var questionId = Guid.NewGuid();
        var dto = new CreateAnswerDto
        {
            Value = 3,
            QuestionId = questionId,
            ChildId = childId,
            PrcId = user.Id
        };

        _questionServiceMock.Setup(s => s.GetAndValidateById(questionId))
            .ReturnsAsync(BuildQuestion());
        _userServiceMock.Setup(s => s.GetAndValidateById(user.Id))
            .ReturnsAsync(user);
        _repositoryMock.Setup(r => r.Create(It.IsAny<Answer>()))
            .Returns(Task.CompletedTask);

        await _sut.Create(dto);

        _repositoryMock.Verify(r => r.Create(It.IsAny<Answer>()), Times.Once);
    }

    [Fact]
    public async Task Create_QuestionNotFound_ShouldThrowNotFoundException()
    {
        var questionId = Guid.NewGuid();
        var dto = new CreateAnswerDto { Value = 3, QuestionId = questionId, ChildId = Guid.NewGuid(), PrcId = Guid.NewGuid() };

        _questionServiceMock.Setup(s => s.GetAndValidateById(questionId))
            .ThrowsAsync(new NotFoundException("Pregunta no encontrada"));

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Create(dto));
        _repositoryMock.Verify(r => r.Create(It.IsAny<Answer>()), Times.Never);
    }

    [Fact]
    public async Task Create_ChildNotBelongingToUser_ShouldThrowNotFoundException()
    {
        var user = new User("Parent", new DateTime(1985, 1, 1), Sex.Male, "parent@domain.com", "pass123");
        var questionId = Guid.NewGuid();
        var dto = new CreateAnswerDto
        {
            Value = 3,
            QuestionId = questionId,
            ChildId = Guid.NewGuid(),
            PrcId = user.Id
        };

        _questionServiceMock.Setup(s => s.GetAndValidateById(questionId))
            .ReturnsAsync(BuildQuestion());
        _userServiceMock.Setup(s => s.GetAndValidateById(user.Id))
            .ReturnsAsync(user);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Create(dto));
        _repositoryMock.Verify(r => r.Create(It.IsAny<Answer>()), Times.Never);
    }

    [Fact]
    public async Task Create_UserNotFound_ShouldThrowNotFoundException()
    {
        var prcId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var dto = new CreateAnswerDto { Value = 2, QuestionId = questionId, ChildId = Guid.NewGuid(), PrcId = prcId };

        _questionServiceMock.Setup(s => s.GetAndValidateById(questionId)).ReturnsAsync(BuildQuestion());
        _userServiceMock.Setup(s => s.GetAndValidateById(prcId))
            .ThrowsAsync(new NotFoundException("Usuario no encontrado"));

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Create(dto));
    }

    [Fact]
    public async Task Create_ValidDto_ShouldCallUpdateScore()
    {
        var user = BuildUserWithChild(out var childId);
        var questionId = Guid.NewGuid();
        var dto = new CreateAnswerDto { Value = 3, QuestionId = questionId, ChildId = childId, PrcId = user.Id };

        _questionServiceMock.Setup(s => s.GetAndValidateById(questionId)).ReturnsAsync(BuildQuestion());
        _userServiceMock.Setup(s => s.GetAndValidateById(user.Id)).ReturnsAsync(user);
        _repositoryMock.Setup(r => r.Create(It.IsAny<Answer>())).Returns(Task.CompletedTask);
        _userServiceMock.Setup(s => s.UpdateScore(user)).Returns(Task.CompletedTask);

        await _sut.Create(dto);

        _userServiceMock.Verify(s => s.UpdateScore(user), Times.Once);
    }

    [Fact]
    public async Task Create_RepositoryThrows_ShouldStillCallUpdateScore()
    {
        var user = BuildUserWithChild(out var childId);
        var questionId = Guid.NewGuid();
        var dto = new CreateAnswerDto { Value = 3, QuestionId = questionId, ChildId = childId, PrcId = user.Id };

        _questionServiceMock.Setup(s => s.GetAndValidateById(questionId)).ReturnsAsync(BuildQuestion());
        _userServiceMock.Setup(s => s.GetAndValidateById(user.Id)).ReturnsAsync(user);
        _repositoryMock.Setup(r => r.Create(It.IsAny<Answer>())).ThrowsAsync(new Exception("DB error"));
        _userServiceMock.Setup(s => s.UpdateScore(user)).Returns(Task.CompletedTask);

        await Assert.ThrowsAsync<Exception>(() => _sut.Create(dto));

        _userServiceMock.Verify(s => s.UpdateScore(user), Times.Once);
    }

    [Fact]
    public async Task Create_ChildNotBelongingToUser_ShouldNotCallUpdateScore()
    {
        var user = new User("Parent", new DateTime(1985, 1, 1), Sex.Male, "parent@domain.com", "pass123");
        var questionId = Guid.NewGuid();
        var dto = new CreateAnswerDto { Value = 3, QuestionId = questionId, ChildId = Guid.NewGuid(), PrcId = user.Id };

        _questionServiceMock.Setup(s => s.GetAndValidateById(questionId)).ReturnsAsync(BuildQuestion());
        _userServiceMock.Setup(s => s.GetAndValidateById(user.Id)).ReturnsAsync(user);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Create(dto));

        _userServiceMock.Verify(s => s.UpdateScore(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Create_QuestionNotFound_ShouldNotCallUpdateScore()
    {
        var questionId = Guid.NewGuid();
        var dto = new CreateAnswerDto { Value = 3, QuestionId = questionId, ChildId = Guid.NewGuid(), PrcId = Guid.NewGuid() };

        _questionServiceMock.Setup(s => s.GetAndValidateById(questionId))
            .ThrowsAsync(new NotFoundException("Pregunta no encontrada"));

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Create(dto));

        _userServiceMock.Verify(s => s.UpdateScore(It.IsAny<User>()), Times.Never);
    }
}
