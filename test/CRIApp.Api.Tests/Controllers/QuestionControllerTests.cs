using Microsoft.Extensions.Logging;
using CRIApp.Api.Controllers;
using CRIApp.Api.Controllers.Requests;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Moq;
using Xunit;

namespace CRIApp.Api.Tests.Controllers;

public class QuestionControllerTests
{
    private readonly Mock<IQuestionService> _serviceMock;
    private readonly QuestionController _sut;
    private readonly Guid _userId = Guid.NewGuid();

    public QuestionControllerTests()
    {
        _serviceMock = new Mock<IQuestionService>();
        var loggerMock = new Mock<ILogger<QuestionController>>();
        _sut = new QuestionController(_serviceMock.Object, loggerMock.Object)
        {
            ControllerContext = ControllerContextFactory.CreateAuthenticated(_userId)
        };
    }

    [Fact]
    public async Task GetQuestionsToReply_ValidRequest_ShouldReturnQuestions()
    {
        var childId = Guid.NewGuid();
        var questions = new List<QuestionDto>
        {
            new QuestionDto(new Domain.Entities.Question("Question 1?", QuestionType.Agreement)),
            new QuestionDto(new Domain.Entities.Question("Question 2?", QuestionType.Frequency))
        };
        var request = new GetQuestionsToReplyRequest { ChildId = childId };
        _serviceMock.Setup(s => s.GetQuestionsToReply(_userId, childId)).ReturnsAsync(questions);

        var result = await _sut.GetQuestionsToReply(request);

        Assert.True(result.Success);
        Assert.Equal(2, result.Data.Count());
        _serviceMock.Verify(s => s.GetQuestionsToReply(_userId, childId), Times.Once);
    }

    [Fact]
    public async Task GetQuestionsToReply_EmptyChildId_ShouldThrowValidationException()
    {
        var request = new GetQuestionsToReplyRequest { ChildId = Guid.Empty };

        await Assert.ThrowsAsync<ValidationException>(() => _sut.GetQuestionsToReply(request));
    }

    [Fact]
    public async Task GetQuestionsToReply_UserNotFound_ShouldPropagateNotFoundException()
    {
        var childId = Guid.NewGuid();
        var request = new GetQuestionsToReplyRequest { ChildId = childId };
        _serviceMock.Setup(s => s.GetQuestionsToReply(_userId, childId))
            .ThrowsAsync(new NotFoundException("Usuario no encontrado"));

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetQuestionsToReply(request));
    }

    [Fact]
    public async Task GetQuestionsToReply_NoQuestionsAvailable_ShouldReturnEmptyData()
    {
        var childId = Guid.NewGuid();
        var request = new GetQuestionsToReplyRequest { ChildId = childId };
        _serviceMock.Setup(s => s.GetQuestionsToReply(_userId, childId))
            .ReturnsAsync(new List<QuestionDto>());

        var result = await _sut.GetQuestionsToReply(request);

        Assert.True(result.Success);
        Assert.Empty(result.Data);
    }
}
