using CRIApp.Api.Controllers;
using CRIApp.Api.Controllers.Requests;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CRIApp.Api.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _serviceMock;
    private readonly Mock<ILogger<AuthController>> _loggerMock;
    private readonly AuthController _sut;

    public AuthControllerTests()
    {
        _serviceMock = new Mock<IAuthService>();
        _loggerMock = new Mock<ILogger<AuthController>>();
        _sut = new AuthController(_serviceMock.Object, _loggerMock.Object)
        {
            ControllerContext = ControllerContextFactory.CreateAuthenticated(Guid.NewGuid())
        };
    }

    private static User BuildUser()
        => new User("Test", new DateTime(1990, 1, 1), Sex.Male, "user@domain.com", "pass123");

    [Fact]
    public async Task Login_ValidCredentials_ShouldReturnTokenInData()
    {
        var user = BuildUser();
        var tokenDto = new TokenDto
        {
            Token = "jwt.token.here",
            ExpiresIn = DateTime.UtcNow.AddMinutes(5),
            User = new UserDto(user)
        };
        var request = new LoginRequest { Username = "user@domain.com", Password = "pass123" };
        _serviceMock.Setup(s => s.Login(It.IsAny<LoginDto>())).ReturnsAsync(tokenDto);

        var result = await _sut.Login(request);

        Assert.True(result.Success);
        Assert.Contains(tokenDto, result.Data);
    }

    [Fact]
    public async Task Login_EmptyUsername_ShouldThrowValidationException()
    {
        var request = new LoginRequest { Username = "", Password = "pass" };

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Login(request));
    }

    [Fact]
    public async Task Login_InvalidUsername_ShouldThrowValidationException()
    {
        var request = new LoginRequest { Username = "not-a-valid-username", Password = "pass" };

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Login(request));
    }

    [Fact]
    public async Task ForgotPassword_ValidUsername_ShouldCallServiceAndReturnSuccess()
    {
        var request = new ForgotPasswordRequest { Username = "user@domain.com" };
        _serviceMock.Setup(s => s.ForgotPassword(request.Username)).Returns(Task.CompletedTask);

        var result = await _sut.ForgotPassword(request);

        Assert.True(result.Success);
        _serviceMock.Verify(s => s.ForgotPassword("user@domain.com"), Times.Once);
    }

    [Fact]
    public async Task ForgotPassword_InvalidUsername_ShouldThrowValidationException()
    {
        var request = new ForgotPasswordRequest { Username = "not-valid" };

        await Assert.ThrowsAsync<ValidationException>(() => _sut.ForgotPassword(request));
    }

    [Fact]
    public async Task ResetPassword_ValidRequest_ShouldCallServiceAndReturnSuccess()
    {
        var request = new ResetPasswordRequest
        {
            Username = "user@domain.com",
            Code = "ABC12",
            NewPassword = "NewPass1!"
        };
        _serviceMock.Setup(s => s.ResetPassword(It.IsAny<ResetPasswordDto>())).Returns(Task.CompletedTask);

        var result = await _sut.ResetPassword(request);

        Assert.True(result.Success);
        _serviceMock.Verify(s => s.ResetPassword(It.IsAny<ResetPasswordDto>()), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_EmptyCode_ShouldThrowValidationException()
    {
        var request = new ResetPasswordRequest
        {
            Username = "user@domain.com",
            Code = "",
            NewPassword = "newpass"
        };

        await Assert.ThrowsAsync<ValidationException>(() => _sut.ResetPassword(request));
    }
}
