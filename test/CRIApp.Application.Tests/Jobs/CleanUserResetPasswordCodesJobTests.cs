using CRIApp.Application.Jobs;
using CRIApp.Domain.Data;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace CRIApp.Application.Tests.Jobs;

public class CleanUserResetPasswordCodesJobTests
{
    private readonly Mock<IUserResetPasswordCodeRepository> _userResetPasswordCodeRepositoryMock;
    private readonly CleanUserResetPasswordCodesJob _sut;

    public CleanUserResetPasswordCodesJobTests()
    {
        _userResetPasswordCodeRepositoryMock = new Mock<IUserResetPasswordCodeRepository>();
        _sut = new CleanUserResetPasswordCodesJob(
            _userResetPasswordCodeRepositoryMock.Object,
            NullLogger<CleanUserResetPasswordCodesJob>.Instance);
    }

    [Fact]
    public async Task Run_ShouldDeleteCodesOlderThanTenMinutes()
    {
        _userResetPasswordCodeRepositoryMock
            .Setup(r => r.DeleteCodesOlderThan(It.IsAny<DateTime>()))
            .ReturnsAsync(3);

        var lowerBound = DateTime.UtcNow.AddMinutes(-10);

        await _sut.Run();

        var upperBound = DateTime.UtcNow.AddMinutes(-10);

        _userResetPasswordCodeRepositoryMock.Verify(
            r => r.DeleteCodesOlderThan(It.Is<DateTime>(threshold => threshold >= lowerBound && threshold <= upperBound)),
            Times.Once);
    }

    [Fact]
    public async Task Run_ShouldCallDeleteOnlyOnce()
    {
        _userResetPasswordCodeRepositoryMock
            .Setup(r => r.DeleteCodesOlderThan(It.IsAny<DateTime>()))
            .ReturnsAsync(0);

        await _sut.Run();

        _userResetPasswordCodeRepositoryMock.Verify(r => r.DeleteCodesOlderThan(It.IsAny<DateTime>()), Times.Once);
    }
}

