using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

public class NotifyUsersWithLessThanTenAnswers
{
    private readonly ILogger<NotifyUsersWithLessThanTenAnswers> _logger;
    public NotifyUsersWithLessThanTenAnswers(ILogger<NotifyUsersWithLessThanTenAnswers> logger)
    {
        _logger = logger;
    }

    public void Run()
    {
        _logger.LogInformation("Job {JobName} started", nameof(NotifyUsersWithLessThanTenAnswers));

        // TODO: query users with fewer than 10 answers today and send notification via Android push notification

        _logger.LogInformation("Job {JobName} completed", nameof(NotifyUsersWithLessThanTenAnswers));
    }
}