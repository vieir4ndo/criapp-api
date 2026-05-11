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

public class UserControllerTests
{
    private readonly Mock<IUserService> _serviceMock;
    private readonly Mock<ILogger<UserController>> _loggerMock;
    private readonly UserController _sut;
    private readonly Guid _userId = Guid.NewGuid();

    public UserControllerTests()
    {
        _serviceMock = new Mock<IUserService>();
        _loggerMock = new Mock<ILogger<UserController>>();
        _sut = new UserController(_serviceMock.Object, _loggerMock.Object)
        {
            ControllerContext = ControllerContextFactory.CreateAuthenticated(_userId)
        };
    }

    private static User BuildUser()
        => new User("Test", new DateTime(1990, 1, 1), Sex.Male, "user@domain.com", "pass123");

    [Fact]
    public async Task Create_ValidRequest_ShouldCallServiceAndReturnSuccess()
    {
        var request = new CreateUserRequest
        {
            Name = "Test User",
            BirthDate = "1990-01-01",
            Sex = Sex.Male,
            Username = "newuser@domain.com",
            Password = "Password1!"
        };
        _serviceMock.Setup(s => s.Create(It.IsAny<CreateUserDto>())).Returns(Task.CompletedTask);

        var result = await _sut.Create(request);

        Assert.True(result.Success);
        _serviceMock.Verify(s => s.Create(It.IsAny<CreateUserDto>()), Times.Once);
    }

    [Fact]
    public async Task Create_InvalidRequest_ShouldThrowValidationException()
    {
        var request = new CreateUserRequest
        {
            Name = "",
            BirthDate = "1990-01-01",
            Sex = Sex.Male,
            Username = "user@domain.com",
            Password = "pass"
        };

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Create(request));
    }

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
    public async Task Get_AuthenticatedUser_ShouldReturnUserDto()
    {
        var user = BuildUser();
        _serviceMock.Setup(s => s.GetAndValidateById(_userId)).ReturnsAsync(user);

        var result = await _sut.Get();

        Assert.True(result.Success);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task Update_ValidRequest_ShouldCallServiceAndReturnSuccess()
    {
        var request = new UpdateUserRequest { Name = "New Name" };
        _serviceMock.Setup(s => s.Update(_userId, It.IsAny<UpdateUserDto>())).Returns(Task.CompletedTask);

        var result = await _sut.Update(request);

        Assert.True(result.Success);
        _serviceMock.Verify(s => s.Update(_userId, It.IsAny<UpdateUserDto>()), Times.Once);
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

    [Fact]
    public async Task Deactivate_AuthenticatedUser_ShouldCallServiceAndReturnSuccess()
    {
        _serviceMock.Setup(s => s.Deactivate(_userId)).Returns(Task.CompletedTask);

        var result = await _sut.Deactivate();

        Assert.True(result.Success);
        _serviceMock.Verify(s => s.Deactivate(_userId), Times.Once);
    }

    [Fact]
    public async Task Deactivate_ServiceThrowsNotFoundException_ShouldPropagate()
    {
        _serviceMock.Setup(s => s.Deactivate(_userId)).ThrowsAsync(new NotFoundException("Usuario no encontrado"));

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Deactivate());
    }
}
