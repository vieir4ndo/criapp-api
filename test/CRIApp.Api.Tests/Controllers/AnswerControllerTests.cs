using CRIApp.Api.Controllers;
using CRIApp.Api.Controllers.Requests;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Exceptions;
using Moq;
using Xunit;

namespace CRIApp.Api.Tests.Controllers;

public class AnswerControllerTests
{
    private readonly Mock<IAnswerService> _serviceMock;
    private readonly AnswerController _sut;
    private readonly Guid _userId = Guid.NewGuid();

    public AnswerControllerTests()
    {
        _serviceMock = new Mock<IAnswerService>();
        _sut = new AnswerController(_serviceMock.Object)
        {
            ControllerContext = ControllerContextFactory.CreateAuthenticated(_userId)
        };
    }

    [Fact]
    public async Task Create_ValidRequest_ShouldCallServiceAndReturnSuccess()
    {
        var questionId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var request = new CreateAnswerRequest { Value = 3, QuestionId = questionId, ChildId = childId };

        _serviceMock.Setup(s => s.Create(It.IsAny<CreateAnswerDto>())).Returns(Task.CompletedTask);

        var result = await _sut.Create(request);

        Assert.True(result.Success);
        _serviceMock.Verify(s => s.Create(It.Is<CreateAnswerDto>(dto =>
            dto.PrcId == _userId &&
            dto.Value == 3 &&
            dto.QuestionId == questionId &&
            dto.ChildId == childId)), Times.Once);
    }

    [Fact]
    public async Task Create_EmptyQuestionId_ShouldThrowValidationException()
    {
        var request = new CreateAnswerRequest { Value = 3, QuestionId = Guid.Empty, ChildId = Guid.NewGuid() };

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Create(request));
    }

    [Fact]
    public async Task Create_EmptyChildId_ShouldThrowValidationException()
    {
        var request = new CreateAnswerRequest { Value = 3, QuestionId = Guid.NewGuid(), ChildId = Guid.Empty };

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Create(request));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(6)]
    public async Task Create_ValueOutOfRange_ShouldThrowValidationException(int value)
    {
        var request = new CreateAnswerRequest { Value = value, QuestionId = Guid.NewGuid(), ChildId = Guid.NewGuid() };

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Create(request));
    }

    [Fact]
    public async Task Create_ChildNotBelongingToUser_ShouldPropagateNotFoundException()
    {
        var request = new CreateAnswerRequest { Value = 3, QuestionId = Guid.NewGuid(), ChildId = Guid.NewGuid() };

        _serviceMock.Setup(s => s.Create(It.IsAny<CreateAnswerDto>()))
            .ThrowsAsync(new NotFoundException("Usuario no tiene niño(a)"));

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Create(request));
    }
}
