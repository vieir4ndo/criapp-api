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

