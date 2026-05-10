using CRIApp.Application.Abstractions;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

public class SendMotivationalNotificationJob : IRecurrentJob
{
    private readonly ILogger<SendMotivationalNotificationJob> _logger;
    private readonly IChildBookRepository _childBookRepository;
    private readonly INotificationService _notificationService;

    public SendMotivationalNotificationJob(
        ILogger<SendMotivationalNotificationJob> logger,
        IChildBookRepository childBookRepository,
        INotificationService notificationService)
    {
        _logger = logger;
        _childBookRepository = childBookRepository;
        _notificationService = notificationService;
    }

    public async Task Run()
    {
        _logger.LogInformation("Job {JobName} started", nameof(SendMotivationalNotificationJob));

        var today = DateTime.UtcNow.Date;
        var childBooks = await _childBookRepository.GetAllForNotifications();

        foreach (var childBook in childBooks)
        {
            if (!ShouldNotify(childBook, today)) continue;

            await _notificationService.CreateIfNotExists(childBook.Id, NotificationType.Motivational);

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
