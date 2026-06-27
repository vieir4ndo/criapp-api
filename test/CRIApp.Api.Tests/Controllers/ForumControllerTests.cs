using Microsoft.Extensions.Logging;
using CRIApp.Api.Controllers;
using CRIApp.Api.Controllers.Requests;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Moq;
using Xunit;

namespace CRIApp.Api.Tests.Controllers;

public class ForumControllerTests
{
    private readonly Mock<IForumService> _serviceMock;
    private readonly Mock<ILogger<ForumController>> _loggerMock;
    private readonly ForumController _sut;
    private readonly Guid _userId = Guid.NewGuid();

    public ForumControllerTests()
    {
        _serviceMock = new Mock<IForumService>();
        _loggerMock = new Mock<ILogger<ForumController>>();
        _sut = new ForumController(_serviceMock.Object, _loggerMock.Object)
        {
            ControllerContext = ControllerContextFactory.CreateAuthenticated(_userId)
        };
    }

    private static ForumDto BuildForumDto(string name = "Forum Title")
    {
        var prc = new User("Parent Name", new DateTime(1985, 1, 1), Sex.Male, "parent@domain.com", "pass123");
        var forum = new Forum(name, "Description", prc.Id);
        typeof(Forum).GetProperty(nameof(Forum.Prc))!.SetValue(forum, prc);
        return new ForumDto(forum);
    }

    private static CommentDto BuildCommentDto()
    {
        var prc = new User("User", new DateTime(1990, 1, 1), Sex.Male, "user@domain.com", "pass123");
        var comment = new Comment("Hello", Guid.NewGuid(), prc.Id);
        typeof(Comment).GetProperty(nameof(Comment.Prc))!.SetValue(comment, prc);
        return new CommentDto(comment);
    }

    [Fact]
    public async Task Create_ValidRequest_ShouldCallServiceAndReturnSuccess()
    {
        var request = new CreateForumRequest { Name = "My Forum", Description = "A description" };
        _serviceMock.Setup(s => s.Create(It.IsAny<CreateForumDto>())).Returns(Task.CompletedTask);

        var result = await _sut.Create(request);

        Assert.True(result.Success);
        _serviceMock.Verify(s => s.Create(It.Is<CreateForumDto>(dto =>
            dto.PrcId == _userId &&
            dto.Name == "My Forum")), Times.Once);
    }

    [Fact]
    public async Task Create_EmptyName_ShouldThrowValidationException()
    {
        var request = new CreateForumRequest { Name = "", Description = "desc" };

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Create(request));
    }

    [Fact]
    public async Task Create_EmptyDescription_ShouldThrowValidationException()
    {
        var request = new CreateForumRequest { Name = "name", Description = "" };

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Create(request));
    }

    [Fact]
    public async Task GetById_ValidForumId_ShouldReturnForumDto()
    {
        var forumId = Guid.NewGuid();
        var forum = BuildForumDto();
        _serviceMock.Setup(s => s.GetById(forumId)).ReturnsAsync(forum);

        var result = await _sut.GetById(forumId);

        Assert.True(result.Success);
        Assert.Contains(forum, result.Data);
    }

    [Fact]
    public async Task GetById_ForumNotFound_ShouldPropagateNotFoundException()
    {
        var forumId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetById(forumId))
            .ThrowsAsync(new NotFoundException("Foro no encontrado"));

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetById(forumId));
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllForums()
    {
        var forums = new List<ForumDto> { BuildForumDto("Forum A"), BuildForumDto("Forum B") };
        _serviceMock.Setup(s => s.GetAll()).ReturnsAsync(forums);

        var result = await _sut.GetAll();

        Assert.True(result.Success);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task Update_ValidRequest_ShouldCallServiceAndReturnSuccess()
    {
        var forumId = Guid.NewGuid();
        var request = new UpdateForumRequest { Name = "Updated Name" };
        _serviceMock.Setup(s => s.Update(forumId, It.IsAny<UpdateForumDto>())).Returns(Task.CompletedTask);

        var result = await _sut.Update(forumId, request);

        Assert.True(result.Success);
        _serviceMock.Verify(s => s.Update(forumId, It.Is<UpdateForumDto>(dto => dto.PrcId == _userId)), Times.Once);
    }

    [Fact]
    public async Task Update_EmptyName_ShouldThrowValidationException()
    {
        var request = new UpdateForumRequest { Name = "" };

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Update(Guid.NewGuid(), request));
    }

    [Fact]
    public async Task GetCommentsById_ShouldReturnComments()
    {
        var forumId = Guid.NewGuid();
        var forum = BuildForumDto();
        forum.Comments = new List<CommentDto> { BuildCommentDto() };
        _serviceMock.Setup(s => s.GetById(forumId)).ReturnsAsync(forum);

        var result = await _sut.GetCommentsById(forumId);

        Assert.True(result.Success);
        Assert.Single(result.Data);
    }
}
