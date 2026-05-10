using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

public class SendMotivationalNotificationJob : IRecurrentJob
{
    private readonly ILogger<SendMotivationalNotificationJob> _logger;
    private readonly IChildBookRepository _childBookRepository;
    private readonly INotificationRepository _notificationRepository;

    public SendMotivationalNotificationJob(
        ILogger<SendMotivationalNotificationJob> logger,
        IChildBookRepository childBookRepository,
        INotificationRepository notificationRepository)
    {
        _logger = logger;
        _childBookRepository = childBookRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task Run()
    {
        _logger.LogInformation("Job {JobName} started", nameof(SendMotivationalNotificationJob));

        var today = DateTime.UtcNow.Date;
        var childBooks = await _childBookRepository.GetAllForNotifications();

        foreach (var childBook in childBooks)
        {
            if (!ShouldNotify(childBook, today)) continue;

            var alreadyExists = await _notificationRepository.ExistsForToday(childBook.Id, NotificationType.Motivational, today);
            if (alreadyExists) continue;

            await _notificationRepository.Create(new Notification(childBook.Id, NotificationType.Motivational));

            _logger.LogInformation("Motivational notification created for ChildBook {ChildBookId}", childBook.Id);
        }

        _logger.LogInformation("Job {JobName} completed", nameof(SendMotivationalNotificationJob));
    }

    public static bool ShouldNotify(ChildBook childBook, DateTime today)
    {
        var lastCheckpointDate = childBook.RecommendationCheckpoints
            .Select(c => c.Date)
            .DefaultIfEmpty()
            .Max();

        if (lastCheckpointDate == default) return false;

        var daysSinceCheckpoint = (today - lastCheckpointDate).Days;
        return daysSinceCheckpoint == 2;
    }
}
