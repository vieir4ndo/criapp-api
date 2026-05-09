using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Application.Resources;
using CRIApp.Application.Services;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using CRIApp.Domain.Settings;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CRIApp.Application.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserResetPasswordCodeRepository> _resetCodeRepositoryMock;
    private readonly Mock<IMessageUser> _messageUserMock;
    private readonly IOptions<JwtTokenSettings> _jwtOptions;
    private readonly Mock<IStringLocalizer<AppMessages>> _localizerMock;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _resetCodeRepositoryMock = new Mock<IUserResetPasswordCodeRepository>();
        _messageUserMock = new Mock<IMessageUser>();

        _jwtOptions = Options.Create(new JwtTokenSettings
        {
            Key = "supersecretkey1234567890abcdefghij",
            Issuer = "criapp",
            ExpiresInMinutes = 5
        });

        _localizerMock = new Mock<IStringLocalizer<AppMessages>>();
        _sut = new UserService(
            _userRepositoryMock.Object,
            _jwtOptions,
            _messageUserMock.Object,
            _resetCodeRepositoryMock.Object,
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
        _userRepositoryMock.Setup(r => r.GetByUsername(dto.Username))
            .ReturnsAsync(BuildUser(dto.Username));

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
        _userRepositoryMock.Setup(r => r.GetByUsername(dto.Username))
            .ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.Create(It.IsAny<User>()))
            .Returns(Task.CompletedTask);
        _messageUserMock.Setup(m => m.NotifyAccountCreation(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        await _sut.Create(dto);

        _userRepositoryMock.Verify(r => r.Create(It.IsAny<User>()), Times.Once);
        _messageUserMock.Verify(m => m.NotifyAccountCreation(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Login_ValidCredentials_ShouldReturnToken()
    {
        var user = BuildUser("user@domain.com", "correctpass");
        var dto = new LoginDto { Username = "user@domain.com", Password = "correctpass" };
        _userRepositoryMock.Setup(r => r.GetByUsername(dto.Username))
            .ReturnsAsync(user);

        var result = await _sut.Login(dto);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
        Assert.NotNull(result.User);
    }

    [Fact]
    public async Task Login_WrongPassword_ShouldThrowValidationException()
    {
        var user = BuildUser("user@domain.com", "correctpass");
        var dto = new LoginDto { Username = "user@domain.com", Password = "wrongpass" };
        _userRepositoryMock.Setup(r => r.GetByUsername(dto.Username))
            .ReturnsAsync(user);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Login(dto));
    }

    [Fact]
    public async Task Login_InactiveUser_ShouldThrowValidationException()
    {
        var user = BuildUser("user@domain.com", "correctpass");
        user.Deactivate();
        var dto = new LoginDto { Username = "user@domain.com", Password = "correctpass" };
        _userRepositoryMock.Setup(r => r.GetByUsername(dto.Username))
            .ReturnsAsync(user);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Login(dto));
    }

    [Fact]
    public async Task Login_UserNotFound_ShouldThrowValidationException()
    {
        var dto = new LoginDto { Username = "nobody@domain.com", Password = "pass" };
        _userRepositoryMock.Setup(r => r.GetByUsername(dto.Username))
            .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Login(dto));
    }

    [Fact]
    public async Task GetAndValidateById_UserExists_ShouldReturnUser()
    {
        var userId = Guid.NewGuid();
        var user = BuildUser();
        _userRepositoryMock.Setup(r => r.GetById(userId))
            .ReturnsAsync(user);

        var result = await _sut.GetAndValidateById(userId);

        Assert.Equal(user, result);
    }

    [Fact]
    public async Task GetAndValidateById_UserNotFound_ShouldThrowNotFoundException()
    {
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(r => r.GetById(userId))
            .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetAndValidateById(userId));
    }

    [Fact]
    public async Task Update_UserExists_ShouldCallUpdateOnRepository()
    {
        var userId = Guid.NewGuid();
        var user = BuildUser();
        var dto = new UpdateUserDto { Name = "Updated Name" };

        _userRepositoryMock.Setup(r => r.GetById(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(r => r.Update(It.IsAny<User>())).Returns(Task.CompletedTask);

        await _sut.Update(userId, dto);

        _userRepositoryMock.Verify(r => r.Update(user), Times.Once);
    }

    [Fact]
    public async Task Update_UserNotFound_ShouldThrowNotFoundException()
    {
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(r => r.GetById(userId)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.Update(userId, new UpdateUserDto { Name = "Name" }));
    }

    [Fact]
    public async Task ForgotPassword_UserNotFound_ShouldThrowValidationException()
    {
        _userRepositoryMock.Setup(r => r.GetByUsername("unknown@domain.com"))
            .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.ForgotPassword("unknown@domain.com"));
    }

    [Fact]
    public async Task ForgotPassword_UserExists_ShouldCreateCodeAndNotify()
    {
        var user = BuildUser();
        _userRepositoryMock.Setup(r => r.GetByUsername("user@domain.com"))
            .ReturnsAsync(user);
        _resetCodeRepositoryMock.Setup(r => r.Create(It.IsAny<UserResetPasswordCode>()))
            .Returns(Task.CompletedTask);
        _messageUserMock.Setup(m => m.NotifyForgotPassword(It.IsAny<User>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        await _sut.ForgotPassword("user@domain.com");

        _resetCodeRepositoryMock.Verify(r => r.Create(It.IsAny<UserResetPasswordCode>()), Times.Once);
        _messageUserMock.Verify(m => m.NotifyForgotPassword(user, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_UserNotFound_ShouldThrowValidationException()
    {
        var dto = new ResetPasswordDto { Username = "ghost@domain.com", Code = "ABC12", NewPassword = "newpass" };
        _userRepositoryMock.Setup(r => r.GetByUsername(dto.Username))
            .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.ResetPassword(dto));
    }

    [Fact]
    public async Task ResetPassword_InvalidCode_ShouldThrowValidationException()
    {
        var user = BuildUser();
        var dto = new ResetPasswordDto { Username = "user@domain.com", Code = "WRONG", NewPassword = "newpass" };
        _userRepositoryMock.Setup(r => r.GetByUsername(dto.Username))
            .ReturnsAsync(user);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.ResetPassword(dto));
    }

    [Fact]
    public async Task GetByUsername_UserExists_ShouldReturnUser()
    {
        var user = BuildUser();
        _userRepositoryMock.Setup(r => r.GetByUsername("user@domain.com")).ReturnsAsync(user);

        var result = await _sut.GetByUsername("user@domain.com");

        Assert.Equal(user, result);
    }

    [Fact]
    public async Task GetByUsername_UserNotFound_ShouldReturnNull()
    {
        _userRepositoryMock.Setup(r => r.GetByUsername("ghost@domain.com")).ReturnsAsync((User?)null);

        var result = await _sut.GetByUsername("ghost@domain.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateScore_ShouldCallUpdateOnRepository()
    {
        var user = BuildUser();
        _userRepositoryMock.Setup(r => r.Update(user)).Returns(Task.CompletedTask);

        await _sut.UpdateScore(user);

        _userRepositoryMock.Verify(r => r.Update(user), Times.Once);
    }

    [Fact]
    public async Task UpdateScore_ShouldUpdateUserScoreBeforePersisting()
    {
        var user = BuildUser();
        User? captured = null;
        _userRepositoryMock.Setup(r => r.Update(It.IsAny<User>()))
            .Callback<User>(u => captured = u)
            .Returns(Task.CompletedTask);

        await _sut.UpdateScore(user);

        Assert.NotNull(captured);
        Assert.Equal(user.Score, captured!.Score);
    }

    [Fact]
    public async Task GetAwards_UserNotFound_ShouldThrowNotFoundException()
    {
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(r => r.GetByIdWithAwardsAndRecommendations(userId))
            .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetAwards(userId));
    }

    [Fact]
    public async Task GetAwards_UserWithNoAwards_ShouldReturnEmptyList()
    {
        var userId = Guid.NewGuid();
        var user = BuildUser();
        _userRepositoryMock.Setup(r => r.GetByIdWithAwardsAndRecommendations(userId))
            .ReturnsAsync(user);

        var result = await _sut.GetAwards(userId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAwards_UserWithAwards_ShouldReturnMappedDtos()
    {
        var userId = Guid.NewGuid();
        var user = BuildUser();
        user.AddAwardIfNeeded(AwardType.FirstSteps);
        user.AddAwardIfNeeded(AwardType.EngagedFamily);
        _userRepositoryMock.Setup(r => r.GetByIdWithAwardsAndRecommendations(userId))
            .ReturnsAsync(user);

        var result = await _sut.GetAwards(userId);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, a => a.Type == AwardType.FirstSteps);
        Assert.Contains(result, a => a.Type == AwardType.EngagedFamily);
    }

    [Fact]
    public async Task GetRecommendations_UserNotFound_ShouldThrowNotFoundException()
    {
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(r => r.GetByIdWithAwardsAndRecommendations(userId))
            .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetRecommendations(userId));
    }

    [Fact]
    public async Task GetRecommendations_UserWithNoRecommendations_ShouldReturnEmptyList()
    {
        var userId = Guid.NewGuid();
        var user = BuildUser();
        _userRepositoryMock.Setup(r => r.GetByIdWithAwardsAndRecommendations(userId))
            .ReturnsAsync(user);

        var result = await _sut.GetRecommendations(userId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Deactivate_UserExists_ShouldSetActiveToFalseAndUpdate()
    {
        var userId = Guid.NewGuid();
        var user = BuildUser();
        user.Activate();
        _userRepositoryMock.Setup(r => r.GetById(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(r => r.Update(It.IsAny<User>())).Returns(Task.CompletedTask);

        await _sut.Deactivate(userId);

        Assert.False(user.Active);
        _userRepositoryMock.Verify(r => r.Update(user), Times.Once);
    }

    [Fact]
    public async Task Deactivate_UserNotFound_ShouldThrowNotFoundException()
    {
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(r => r.GetById(userId)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Deactivate(userId));
    }
}
