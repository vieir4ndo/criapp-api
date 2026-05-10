using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

public class SendReEvaluationNotificationJob : IRecurrentJob
{
    private readonly ILogger<SendReEvaluationNotificationJob> _logger;
    private readonly IChildBookRepository _childBookRepository;
    private readonly INotificationRepository _notificationRepository;

    public SendReEvaluationNotificationJob(
        ILogger<SendReEvaluationNotificationJob> logger,
        IChildBookRepository childBookRepository,
        INotificationRepository notificationRepository)
    {
        _logger = logger;
        _childBookRepository = childBookRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task Run()
    {
        _logger.LogInformation("Job {JobName} started", nameof(SendReEvaluationNotificationJob));

        var today = DateTime.UtcNow.Date;
        var childBooks = await _childBookRepository.GetAllForNotifications();

        foreach (var childBook in childBooks)
        {
            if (!ShouldNotify(childBook, today)) continue;

            var alreadyExists = await _notificationRepository.ExistsForToday(childBook.Id, NotificationType.ReEvaluation, today);
            if (alreadyExists) continue;

            await _notificationRepository.Create(new Notification(childBook.Id, NotificationType.ReEvaluation));

            _logger.LogInformation("ReEvaluation notification created for ChildBook {ChildBookId}", childBook.Id);
        }

        _logger.LogInformation("Job {JobName} completed", nameof(SendReEvaluationNotificationJob));
    }

    public static bool ShouldNotify(ChildBook childBook, DateTime today)
    {
        var lastCheckpointDate = childBook.RecommendationCheckpoints
            .Select(c => c.Date)
            .DefaultIfEmpty()
            .Max();

        var lastAnswerDate = childBook.Child.Answers
            .Select(a => a.CreatedAt.Date)
            .DefaultIfEmpty()
            .Max();

        var lastActivityDate = lastCheckpointDate > lastAnswerDate ? lastCheckpointDate : lastAnswerDate;

        if (lastActivityDate == default) return true;

        return (today - lastActivityDate).Days >= 7;
    }
}
