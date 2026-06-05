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

public class AuthServiceTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IUserResetPasswordCodeRepository> _resetCodeRepositoryMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IMessageUser> _messageUserMock;
    private readonly Mock<IStringLocalizer<AppMessages>> _localizerMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _resetCodeRepositoryMock = new Mock<IUserResetPasswordCodeRepository>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _messageUserMock = new Mock<IMessageUser>();
        _localizerMock = new Mock<IStringLocalizer<AppMessages>>();

        var jwtOptions = Options.Create(new JwtTokenSettings
        {
            Key = "supersecretkey1234567890abcdefghij",
            Issuer = "criapp",
            ExpiresInMinutes = 5,
            RefreshTokenExpiresInDays = 30
        });

        _sut = new AuthService(
            _userServiceMock.Object,
            _resetCodeRepositoryMock.Object,
            _refreshTokenRepositoryMock.Object,
            _messageUserMock.Object,
            jwtOptions,
            NullLogger<AuthService>.Instance,
            _localizerMock.Object);
    }

    private static User BuildUser(string username = "user@domain.com", string password = "pass123")
        => new User("Test User", new DateTime(1990, 1, 1), Sex.Male, username, password);

    [Fact]
    public async Task Login_ValidCredentials_ShouldReturnToken()
    {
        var user = BuildUser("user@domain.com", "correctpass");
        var dto = new LoginDto { Username = "user@domain.com", Password = "correctpass" };
        _userServiceMock.Setup(s => s.GetByUsername(dto.Username)).ReturnsAsync(user);
        _refreshTokenRepositoryMock.Setup(r => r.Create(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);

        var result = await _sut.Login(dto);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
        Assert.NotEmpty(result.RefreshToken);
        Assert.NotNull(result.User);
    }

    [Fact]
    public async Task Login_WrongPassword_ShouldThrowValidationException()
    {
        var user = BuildUser("user@domain.com", "correctpass");
        var dto = new LoginDto { Username = "user@domain.com", Password = "wrongpass" };
        _userServiceMock.Setup(s => s.GetByUsername(dto.Username)).ReturnsAsync(user);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Login(dto));
    }

    [Fact]
    public async Task Login_InactiveUser_ShouldThrowValidationException()
    {
        var user = BuildUser("user@domain.com", "correctpass");
        user.Deactivate();
        var dto = new LoginDto { Username = "user@domain.com", Password = "correctpass" };
        _userServiceMock.Setup(s => s.GetByUsername(dto.Username)).ReturnsAsync(user);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Login(dto));
    }

    [Fact]
    public async Task Login_UserNotFound_ShouldThrowValidationException()
    {
        var dto = new LoginDto { Username = "nobody@domain.com", Password = "pass" };
        _userServiceMock.Setup(s => s.GetByUsername(dto.Username)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Login(dto));
    }

    [Fact]
    public async Task ForgotPassword_UserNotFound_ShouldThrowValidationException()
    {
        _userServiceMock.Setup(s => s.GetByUsername("unknown@domain.com")).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.ForgotPassword("unknown@domain.com"));
    }

    [Fact]
    public async Task ForgotPassword_UserExists_ShouldCreateCodeAndNotify()
    {
        var user = BuildUser();
        _userServiceMock.Setup(s => s.GetByUsername("user@domain.com")).ReturnsAsync(user);
        _resetCodeRepositoryMock.Setup(r => r.Create(It.IsAny<UserResetPasswordCode>())).Returns(Task.CompletedTask);
        _messageUserMock.Setup(m => m.NotifyForgotPassword(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        await _sut.ForgotPassword("user@domain.com");

        _resetCodeRepositoryMock.Verify(r => r.Create(It.IsAny<UserResetPasswordCode>()), Times.Once);
        _messageUserMock.Verify(m => m.NotifyForgotPassword(user, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_UserNotFound_ShouldThrowValidationException()
    {
        var dto = new ResetPasswordDto { Username = "ghost@domain.com", Code = "ABC12", NewPassword = "newpass" };
        _userServiceMock.Setup(s => s.GetByUsername(dto.Username)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.ResetPassword(dto));
    }

    [Fact]
    public async Task ResetPassword_InvalidCode_ShouldThrowValidationException()
    {
        var user = BuildUser();
        var dto = new ResetPasswordDto { Username = "user@domain.com", Code = "WRONG", NewPassword = "newpass" };
        _userServiceMock.Setup(s => s.GetByUsername(dto.Username)).ReturnsAsync(user);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.ResetPassword(dto));
    }
}

