using CRIApp.Domain.Data;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

public class CleanExpiredRefreshTokensJob : IRecurrentJob
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ILogger<CleanExpiredRefreshTokensJob> _logger;

    public CleanExpiredRefreshTokensJob(
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<CleanExpiredRefreshTokensJob> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }

    public async Task Run()
    {
        _logger.LogInformation("Job {JobName} started", nameof(CleanExpiredRefreshTokensJob));

        var removed = await _refreshTokenRepository.DeleteExpiredAsync();

        _logger.LogInformation(
            "Job {JobName} completed. Removed {Removed} expired refresh tokens",
            nameof(CleanExpiredRefreshTokensJob),
            removed);
    }
}
