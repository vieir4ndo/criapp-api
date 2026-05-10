using CRIApp.Application.Abstractions;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

public class SendReEvaluationNotificationJob : IRecurrentJob
{
    private readonly ILogger<SendReEvaluationNotificationJob> _logger;
    private readonly IChildBookRepository _childBookRepository;
    private readonly INotificationService _notificationService;

    public SendReEvaluationNotificationJob(
        ILogger<SendReEvaluationNotificationJob> logger,
        IChildBookRepository childBookRepository,
        INotificationService notificationService)
    {
        _logger = logger;
        _childBookRepository = childBookRepository;
        _notificationService = notificationService;
    }

    public async Task Run()
    {
        _logger.LogInformation("Job {JobName} started", nameof(SendReEvaluationNotificationJob));

        var today = DateTime.UtcNow.Date;
        var childBooks = await _childBookRepository.GetAllForNotifications();

        foreach (var childBook in childBooks)
        {
            if (!ShouldNotify(childBook, today)) continue;

            await _notificationService.CreateIfNotExists(childBook.Id, NotificationType.ReEvaluation);

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

        if (lastActivityDate == default) return false;

        return (today - lastActivityDate).Days >= 7;
    }
}
