using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

<<<<<<<< HEAD:src/CRIApp.Application/Jobs/NotifyUsersWithLessThanTenAnswers.cs
[ExcludeFromCodeCoverage]
public class NotifyUsersWithLessThanTenAnswers
========
public class NotifyUsersWithLessThanTenAnswersJob : IRecurrentJob
>>>>>>>> origin/issue#64:src/CRIApp.Application/Jobs/NotifyUsersWithLessThanTenAnswersJob.cs
{
    private readonly ILogger<NotifyUsersWithLessThanTenAnswersJob> _logger;
    public NotifyUsersWithLessThanTenAnswersJob(ILogger<NotifyUsersWithLessThanTenAnswersJob> logger)
    {
        _logger = logger;
    }

    public async Task Run()
    {
<<<<<<<< HEAD:src/CRIApp.Application/Jobs/NotifyUsersWithLessThanTenAnswers.cs
        _logger.LogInformation("Hello World!");
========
        _logger.LogInformation("Job {JobName} started", nameof(NotifyUsersWithLessThanTenAnswersJob));

        // TODO: query users with fewer than 10 answers today and send notification via Android push notification

        _logger.LogInformation("Job {JobName} completed", nameof(NotifyUsersWithLessThanTenAnswersJob));
>>>>>>>> origin/issue#64:src/CRIApp.Application/Jobs/NotifyUsersWithLessThanTenAnswersJob.cs
    }
}