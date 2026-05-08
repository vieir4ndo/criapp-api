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

public class ForumServiceTests
{
    private readonly Mock<IForumRepository> _forumRepositoryMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IStringLocalizer<AppMessages>> _localizerMock;
    private readonly ForumService _sut;

    public ForumServiceTests()
    {
        _forumRepositoryMock = new Mock<IForumRepository>();
        _userServiceMock = new Mock<IUserService>();
        _localizerMock = new Mock<IStringLocalizer<AppMessages>>();
        _sut = new ForumService(_forumRepositoryMock.Object, _userServiceMock.Object, NullLogger<ForumService>.Instance, _localizerMock.Object);
    }

    private static User BuildUser()
        => new User("Parent", new DateTime(1985, 1, 1), Sex.Male, "parent@domain.com", "pass123");

    private static Forum BuildForumWithPrc(Guid prcId)
    {
        var user = new User("Parent", new DateTime(1985, 1, 1), Sex.Male, "parent@domain.com", "pass123");
        var forum = new Forum("Forum Title", "Forum Description", prcId);
        typeof(Forum).GetProperty("Prc")!.SetValue(forum, user);
        return forum;
    }

    [Fact]
    public async Task Create_ValidDto_ShouldCreateForum()
    {
        var prcId = Guid.NewGuid();
        var user = BuildUser();
        var dto = new CreateForumDto { Name = "Forum", Description = "Desc", PrcId = prcId };

        _userServiceMock.Setup(s => s.GetAndValidateById(prcId)).ReturnsAsync(user);
        _forumRepositoryMock.Setup(r => r.Create(It.IsAny<Forum>())).Returns(Task.CompletedTask);

        await _sut.Create(dto);

        _forumRepositoryMock.Verify(r => r.Create(It.IsAny<Forum>()), Times.Once);
    }

    [Fact]
    public async Task Create_UserNotFound_ShouldThrowNotFoundException()
    {
        var prcId = Guid.NewGuid();
        var dto = new CreateForumDto { Name = "Forum", Description = "Desc", PrcId = prcId };

        _userServiceMock.Setup(s => s.GetAndValidateById(prcId))
            .ThrowsAsync(new NotFoundException("Usuario no encontrado"));

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Create(dto));
    }

    [Fact]
    public async Task GetById_ForumExists_ShouldReturnForumDto()
    {
        var forumId = Guid.NewGuid();
        var prcId = Guid.NewGuid();
        var forum = BuildForumWithPrc(prcId);

        _forumRepositoryMock.Setup(r => r.GetById(forumId)).ReturnsAsync(forum);

        var result = await _sut.GetById(forumId);

        Assert.NotNull(result);
        Assert.Equal(forum.Name, result.Name);
    }

    [Fact]
    public async Task GetById_ForumNotFound_ShouldThrowNotFoundException()
    {
        var forumId = Guid.NewGuid();
        _forumRepositoryMock.Setup(r => r.GetById(forumId)).ReturnsAsync((Forum?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetById(forumId));
    }

    [Fact]
    public async Task GetAll_ShouldReturnMappedDtos()
    {
        var prcId = Guid.NewGuid();
        var forums = new List<Forum>
        {
            BuildForumWithPrc(prcId),
            BuildForumWithPrc(prcId)
        };
        _forumRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(forums);

        var result = await _sut.GetAll();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAll_NoForums_ShouldReturnEmptyList()
    {
        _forumRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Forum>());

        var result = await _sut.GetAll();

        Assert.Empty(result);
    }

    [Fact]
    public async Task Update_ValidOwner_ShouldCallUpdateOnRepository()
    {
        var forumId = Guid.NewGuid();
        var prcId = Guid.NewGuid();
        var user = BuildUser();
        var forum = BuildForumWithPrc(prcId);
        var dto = new UpdateForumDto { Name = "New Name", Description = "New Desc", PrcId = prcId };

        _userServiceMock.Setup(s => s.GetAndValidateById(prcId)).ReturnsAsync(user);
        _forumRepositoryMock.Setup(r => r.GetById(forumId)).ReturnsAsync(forum);
        _forumRepositoryMock.Setup(r => r.Update(It.IsAny<Forum>())).Returns(Task.CompletedTask);

        await _sut.Update(forumId, dto);

        _forumRepositoryMock.Verify(r => r.Update(forum), Times.Once);
    }

    [Fact]
    public async Task Update_ForumNotFound_ShouldThrowNotFoundException()
    {
        var forumId = Guid.NewGuid();
        var prcId = Guid.NewGuid();
        var user = BuildUser();
        var dto = new UpdateForumDto { PrcId = prcId };

        _userServiceMock.Setup(s => s.GetAndValidateById(prcId)).ReturnsAsync(user);
        _forumRepositoryMock.Setup(r => r.GetById(forumId)).ReturnsAsync((Forum?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Update(forumId, dto));
    }

    [Fact]
    public async Task Update_ForumBelongsToAnotherUser_ShouldThrowValidationException()
    {
        var forumId = Guid.NewGuid();
        var prcId = Guid.NewGuid();
        var otherPrcId = Guid.NewGuid();
        var user = BuildUser();
        var forum = BuildForumWithPrc(otherPrcId);
        var dto = new UpdateForumDto { PrcId = prcId };

        _userServiceMock.Setup(s => s.GetAndValidateById(prcId)).ReturnsAsync(user);
        _forumRepositoryMock.Setup(r => r.GetById(forumId)).ReturnsAsync(forum);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Update(forumId, dto));
    }
}
