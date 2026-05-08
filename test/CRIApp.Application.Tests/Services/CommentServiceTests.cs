using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
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

public class CommentServiceTests
{
    private readonly Mock<IForumService> _forumServiceMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly Mock<IStringLocalizer<AppMessages>> _localizerMock;
    private readonly CommentService _sut;

    public CommentServiceTests()
    {
        _forumServiceMock = new Mock<IForumService>();
        _userServiceMock = new Mock<IUserService>();
        _commentRepositoryMock = new Mock<ICommentRepository>();
        _localizerMock = new Mock<IStringLocalizer<AppMessages>>();
        _sut = new CommentService(_forumServiceMock.Object, _userServiceMock.Object, _commentRepositoryMock.Object, NullLogger<CommentService>.Instance, _localizerMock.Object);
    }

    private static User BuildUser()
        => new User("Parent", new DateTime(1985, 1, 1), Sex.Male, "parent@domain.com", "pass123");

    private static Comment BuildComment(Guid forumId, Guid prcId)
        => new Comment("Some text", forumId, prcId);

    [Fact]
    public async Task Create_ValidDto_ShouldCreateComment()
    {
        var prcId = Guid.NewGuid();
        var forumId = Guid.NewGuid();
        var user = BuildUser();
        var forumDto = new ForumDto { Id = forumId, Name = "Forum", Description = "Desc", Comments = new() };
        var dto = new CreateCommentDto { Text = "Hello!", PrcId = prcId, ForumId = forumId };

        _userServiceMock.Setup(s => s.GetAndValidateById(prcId)).ReturnsAsync(user);
        _forumServiceMock.Setup(s => s.GetById(forumId)).ReturnsAsync(forumDto);
        _commentRepositoryMock.Setup(r => r.Create(It.IsAny<Comment>())).Returns(Task.CompletedTask);

        await _sut.Create(dto);

        _commentRepositoryMock.Verify(r => r.Create(It.IsAny<Comment>()), Times.Once);
    }

    [Fact]
    public async Task Create_UserNotFound_ShouldThrowNotFoundException()
    {
        var prcId = Guid.NewGuid();
        var dto = new CreateCommentDto { Text = "Hello!", PrcId = prcId, ForumId = Guid.NewGuid() };

        _userServiceMock.Setup(s => s.GetAndValidateById(prcId))
            .ThrowsAsync(new NotFoundException("Usuario no encontrado"));

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Create(dto));
        _commentRepositoryMock.Verify(r => r.Create(It.IsAny<Comment>()), Times.Never);
    }

    [Fact]
    public async Task Create_ForumNotFound_ShouldThrowNotFoundException()
    {
        var prcId = Guid.NewGuid();
        var forumId = Guid.NewGuid();
        var user = BuildUser();
        var dto = new CreateCommentDto { Text = "Hello!", PrcId = prcId, ForumId = forumId };

        _userServiceMock.Setup(s => s.GetAndValidateById(prcId)).ReturnsAsync(user);
        _forumServiceMock.Setup(s => s.GetById(forumId))
            .ThrowsAsync(new NotFoundException("Foro no encontrado"));

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Create(dto));
    }

    [Fact]
    public async Task Delete_CommentExists_ShouldCallDelete()
    {
        var commentId = Guid.NewGuid();
        var comment = BuildComment(Guid.NewGuid(), Guid.NewGuid());

        _commentRepositoryMock.Setup(r => r.GetById(commentId)).ReturnsAsync(comment);
        _commentRepositoryMock.Setup(r => r.Delete(comment)).Returns(Task.CompletedTask);

        await _sut.Delete(commentId);

        _commentRepositoryMock.Verify(r => r.Delete(comment), Times.Once);
    }

    [Fact]
    public async Task Delete_CommentNotFound_ShouldThrowNotFoundException()
    {
        var commentId = Guid.NewGuid();
        _commentRepositoryMock.Setup(r => r.GetById(commentId)).ReturnsAsync((Comment?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Delete(commentId));
        _commentRepositoryMock.Verify(r => r.Delete(It.IsAny<Comment>()), Times.Never);
    }
}
