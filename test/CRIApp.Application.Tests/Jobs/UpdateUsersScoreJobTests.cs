using CRIApp.Application.Abstractions;
using CRIApp.Application.Jobs;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace CRIApp.Application.Tests.Jobs;

public class UpdateUsersScoreJobTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UpdateUsersScoreJob _sut;

    public UpdateUsersScoreJobTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _sut = new UpdateUsersScoreJob(NullLogger<UpdateUsersScoreJob>.Instance, _userServiceMock.Object);
    }

    private static User BuildUser()
        => new User("Parent", new DateTime(1985, 1, 1), Sex.Male, "parent@domain.com", "pass123", FamilyType.HeterosexualCouple);

    [Fact]
    public async Task Run_NoUsers_ShouldNotCallUpdateScore()
    {
        _userServiceMock.Setup(s => s.GetAllUsers()).ReturnsAsync(new List<User>());

        await _sut.Run();

        _userServiceMock.Verify(s => s.UpdateScore(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Run_WithUsers_ShouldCallUpdateScoreForEachUser()
    {
        var users = new List<User> { BuildUser(), BuildUser(), BuildUser() };
        _userServiceMock.Setup(s => s.GetAllUsers()).ReturnsAsync(users);
        _userServiceMock.Setup(s => s.UpdateScore(It.IsAny<User>())).Returns(Task.CompletedTask);

        await _sut.Run();

        _userServiceMock.Verify(s => s.UpdateScore(It.IsAny<User>()), Times.Exactly(3));
    }

    [Fact]
    public async Task Run_WithUsers_ShouldCallUpdateScoreWithCorrectUser()
    {
        var user = BuildUser();
        _userServiceMock.Setup(s => s.GetAllUsers()).ReturnsAsync(new List<User> { user });
        _userServiceMock.Setup(s => s.UpdateScore(user)).Returns(Task.CompletedTask);

        await _sut.Run();

        _userServiceMock.Verify(s => s.UpdateScore(user), Times.Once);
    }

    [Fact]
    public async Task Run_ShouldCallGetAllUsersOnce()
    {
        _userServiceMock.Setup(s => s.GetAllUsers()).ReturnsAsync(new List<User> { BuildUser() });
        _userServiceMock.Setup(s => s.UpdateScore(It.IsAny<User>())).Returns(Task.CompletedTask);

        await _sut.Run();

        _userServiceMock.Verify(s => s.GetAllUsers(), Times.Once);
    }
}
