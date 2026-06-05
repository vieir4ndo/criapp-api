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

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IMessageUser> _messageUserMock;
    private readonly Mock<IStringLocalizer<AppMessages>> _localizerMock;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _messageUserMock = new Mock<IMessageUser>();
        _localizerMock = new Mock<IStringLocalizer<AppMessages>>();

        _sut = new UserService(
            _userRepositoryMock.Object,
            _refreshTokenRepositoryMock.Object,
            _messageUserMock.Object,
            NullLogger<UserService>.Instance,
            _localizerMock.Object);
    }

    private static User BuildUser(string username = "user@domain.com", string password = "pass123")
        => new User("Test User", new DateTime(1990, 1, 1), Sex.Male, username, password);

    [Fact]
    public async Task Create_UsernameAlreadyExists_ShouldThrowValidationException()
    {
        var dto = new CreateUserDto
        {
            Name = "Test",
            BirthDate = new DateTime(1990, 1, 1),
            Sex = Sex.Male,
            Username = "existing@domain.com",
            Password = "password"
        };
        _userRepositoryMock.Setup(r => r.GetByUsername(dto.Username)).ReturnsAsync(BuildUser(dto.Username));

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Create(dto));
    }

    [Fact]
    public async Task Create_NewUsername_ShouldCreateAndNotify()
    {
        var dto = new CreateUserDto
        {
            Name = "New User",
            BirthDate = new DateTime(1990, 1, 1),
            Sex = Sex.Male,
            Username = "new@domain.com",
            Password = "password123"
        };
        _userRepositoryMock.Setup(r => r.GetByUsername(dto.Username)).ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.Create(It.IsAny<User>())).Returns(Task.CompletedTask);
        _messageUserMock.Setup(m => m.NotifyAccountCreation(It.IsAny<User>())).Returns(Task.CompletedTask);

        await _sut.Create(dto);

        _userRepositoryMock.Verify(r => r.Create(It.IsAny<User>()), Times.Once);
        _messageUserMock.Verify(m => m.NotifyAccountCreation(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task GetAndValidateById_UserExists_ShouldReturnUser()
    {
        var userId = Guid.NewGuid();
        var user = BuildUser();
        _userRepositoryMock.Setup(r => r.GetById(userId)).ReturnsAsync(user);

        var result = await _sut.GetAndValidateById(userId);

        Assert.Equal(user, result);
    }

    [Fact]
    public async Task GetAndValidateById_UserNotFound_ShouldThrowNotFoundException()
    {
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(r => r.GetById(userId)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetAndValidateById(userId));
    }

    [Fact]
    public async Task Update_UserExists_ShouldCallUpdateOnRepository()
    {
        var userId = Guid.NewGuid();
        var user = BuildUser();
        _userRepositoryMock.Setup(r => r.GetById(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(r => r.Update(It.IsAny<User>())).Returns(Task.CompletedTask);

        await _sut.Update(userId, new UpdateUserDto { Name = "Updated Name" });

        _userRepositoryMock.Verify(r => r.Update(user), Times.Once);
    }

    [Fact]
    public async Task Update_UserNotFound_ShouldThrowNotFoundException()
    {
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(r => r.GetById(userId)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Update(userId, new UpdateUserDto { Name = "Name" }));
    }

    [Fact]
    public async Task Deactivate_UserExists_ShouldDeactivateAndRevokeTokens()
    {
        var userId = Guid.NewGuid();
        var user = BuildUser();
        user.Activate();
        _userRepositoryMock.Setup(r => r.GetById(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(r => r.Update(It.IsAny<User>())).Returns(Task.CompletedTask);
        _refreshTokenRepositoryMock.Setup(r => r.DeleteByUserId(userId)).ReturnsAsync(0);

        await _sut.Deactivate(userId);

        Assert.False(user.Active);
        _userRepositoryMock.Verify(r => r.Update(user), Times.Once);
        _refreshTokenRepositoryMock.Verify(r => r.DeleteByUserId(userId), Times.Once);
    }

    [Fact]
    public async Task Deactivate_UserNotFound_ShouldThrowNotFoundException()
    {
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(r => r.GetById(userId)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Deactivate(userId));
    }
}

