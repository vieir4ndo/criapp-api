using CRIApp.Application.Jobs;
using CRIApp.Domain.Data;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace CRIApp.Application.Tests.Jobs;

public class CleanExpiredRefreshTokensJobTests
{
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly CleanExpiredRefreshTokensJob _sut;

    public CleanExpiredRefreshTokensJobTests()
    {
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _sut = new CleanExpiredRefreshTokensJob(
            _refreshTokenRepositoryMock.Object,
            NullLogger<CleanExpiredRefreshTokensJob>.Instance);
    }

    [Fact]
    public async Task Run_ShouldCallDeleteExpiredAsync()
    {
        _refreshTokenRepositoryMock.Setup(r => r.DeleteExpiredAsync()).ReturnsAsync(3);

        await _sut.Run();

        _refreshTokenRepositoryMock.Verify(r => r.DeleteExpiredAsync(), Times.Once);
    }

    [Fact]
    public async Task Run_WhenNoExpiredTokens_ShouldComplete()
    {
        _refreshTokenRepositoryMock.Setup(r => r.DeleteExpiredAsync()).ReturnsAsync(0);

        var ex = await Record.ExceptionAsync(() => _sut.Run());

        Assert.Null(ex);
    }
}
