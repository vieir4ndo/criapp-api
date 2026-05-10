using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

public class SendPointsRiskNotificationJob : IRecurrentJob
{
    private readonly ILogger<SendPointsRiskNotificationJob> _logger;
    private readonly IChildBookRepository _childBookRepository;
    private readonly INotificationRepository _notificationRepository;

    public SendPointsRiskNotificationJob(
        ILogger<SendPointsRiskNotificationJob> logger,
        IChildBookRepository childBookRepository,
        INotificationRepository notificationRepository)
    {
        _logger = logger;
        _childBookRepository = childBookRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task Run()
    {
        _logger.LogInformation("Job {JobName} started", nameof(SendPointsRiskNotificationJob));

        var today = DateTime.UtcNow.Date;
        var childBooks = await _childBookRepository.GetAllForNotifications();

        foreach (var childBook in childBooks)
        {
            if (!ShouldNotify(childBook, today)) continue;

            var alreadyExists = await _notificationRepository.ExistsForToday(childBook.Id, NotificationType.PointsRisk, today);
            if (alreadyExists) continue;

            await _notificationRepository.Create(new Notification(childBook.Id, NotificationType.PointsRisk));

            _logger.LogInformation("PointsRisk notification created for ChildBook {ChildBookId}", childBook.Id);
        }

        _logger.LogInformation("Job {JobName} completed", nameof(SendPointsRiskNotificationJob));
    }

    public static bool ShouldNotify(ChildBook childBook, DateTime today)
    {
        var lastCheckpointDate = childBook.RecommendationCheckpoints
            .Select(c => c.Date)
            .DefaultIfEmpty()
            .Max();

        if (lastCheckpointDate == default) return false;

        var daysSinceCheckpoint = (today - lastCheckpointDate).Days;
        return daysSinceCheckpoint == 3;
    }
}
