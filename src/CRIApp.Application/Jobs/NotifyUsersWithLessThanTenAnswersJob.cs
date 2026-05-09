using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

public class NotifyUsersWithLessThanTenAnswersJob : IRecurrentJob
{
    private readonly ILogger<NotifyUsersWithLessThanTenAnswersJob> _logger;
    
    public NotifyUsersWithLessThanTenAnswersJob(ILogger<NotifyUsersWithLessThanTenAnswersJob> logger)
    {
        _logger = logger;
    }

    public Task Run()
    {
        _logger.LogInformation("Job {JobName} started", nameof(NotifyUsersWithLessThanTenAnswersJob));

        // TODO: query users with fewer than 10 answers today and send notification via Android push notification

        _logger.LogInformation("Job {JobName} completed", nameof(NotifyUsersWithLessThanTenAnswersJob));
        
        return Task.CompletedTask;
    }
}