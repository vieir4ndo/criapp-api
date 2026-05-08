using CRIApp.Domain.Data;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

public class CleanUserResetPasswordCodesJob : IRecurrentJob
{
    private readonly IUserResetPasswordCodeRepository _userResetPasswordCodeRepository;
    private readonly ILogger<CleanUserResetPasswordCodesJob> _logger;

    public CleanUserResetPasswordCodesJob(
        IUserResetPasswordCodeRepository userResetPasswordCodeRepository,
        ILogger<CleanUserResetPasswordCodesJob> logger)
    {
        _userResetPasswordCodeRepository = userResetPasswordCodeRepository;
        _logger = logger;
    }

    public async Task Run()
    {
        _logger.LogInformation("Job {JobName} started", nameof(CleanUserResetPasswordCodesJob));

        var threshold = DateTime.UtcNow.AddMinutes(-10);
        var removedCodes = await _userResetPasswordCodeRepository.DeleteCodesOlderThan(threshold);

        _logger.LogInformation(
            "Job {JobName} completed. Removed {RemovedCodes} reset password codes older than {Threshold}",
            nameof(CleanUserResetPasswordCodesJob),
            removedCodes,
            threshold);
    }
}

