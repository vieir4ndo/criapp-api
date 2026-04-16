using CRIApp.Api.Controllers;
using CRIApp.Api.Controllers.Requests;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Exceptions;
using Moq;
using Xunit;

namespace CRIApp.Api.Tests.Controllers;

public class CommentControllerTests
{
    private readonly Mock<ICommentService> _serviceMock;
    private readonly CommentController _sut;
    private readonly Guid _userId = Guid.NewGuid();

    public CommentControllerTests()
    {
        _serviceMock = new Mock<ICommentService>();
        _sut = new CommentController(_serviceMock.Object)
        {
            ControllerContext = ControllerContextFactory.CreateAuthenticated(_userId)
        };
    }

    [Fact]
    public async Task Create_ValidRequest_ShouldCallServiceAndReturnSuccess()
    {
        var forumId = Guid.NewGuid();
        var request = new CreateCommentRequest { Text = "A comment", ForumId = forumId };
        _serviceMock.Setup(s => s.Create(It.IsAny<CreateCommentDto>())).Returns(Task.CompletedTask);

        var result = await _sut.Create(request);

        Assert.True(result.Success);
        _serviceMock.Verify(s => s.Create(It.Is<CreateCommentDto>(dto =>
            dto.PrcId == _userId &&
            dto.ForumId == forumId &&
            dto.Text == "A comment")), Times.Once);
    }

    [Fact]
    public async Task Create_EmptyText_ShouldThrowValidationException()
    {
        var request = new CreateCommentRequest { Text = "", ForumId = Guid.NewGuid() };

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Create(request));
    }

    [Fact]
    public async Task Create_EmptyForumId_ShouldThrowValidationException()
    {
        var request = new CreateCommentRequest { Text = "text", ForumId = Guid.Empty };

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Create(request));
    }

    [Fact]
    public async Task Create_ForumNotFound_ShouldPropagateNotFoundException()
    {
        var request = new CreateCommentRequest { Text = "text", ForumId = Guid.NewGuid() };
        _serviceMock.Setup(s => s.Create(It.IsAny<CreateCommentDto>()))
            .ThrowsAsync(new NotFoundException("Foro no encontrado"));

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Create(request));
    }

    [Fact]
    public async Task Delete_ValidCommentId_ShouldCallServiceAndReturnSuccess()
    {
        var commentId = Guid.NewGuid();
        _serviceMock.Setup(s => s.Delete(commentId)).Returns(Task.CompletedTask);

        var result = await _sut.Delete(commentId);

        Assert.True(result.Success);
        _serviceMock.Verify(s => s.Delete(commentId), Times.Once);
    }

    [Fact]
    public async Task Delete_CommentNotFound_ShouldPropagateNotFoundException()
    {
        var commentId = Guid.NewGuid();
        _serviceMock.Setup(s => s.Delete(commentId))
            .ThrowsAsync(new NotFoundException("Comentario no encontrado"));

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Delete(commentId));
    }
}
