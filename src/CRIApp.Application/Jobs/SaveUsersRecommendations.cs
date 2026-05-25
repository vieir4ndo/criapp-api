using CRIApp.Application.Abstractions;
using CRIApp.Domain.Data;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

public class SaveUsersRecommendations : IRecurrentJob
{
    private readonly ILogger<SaveUsersRecommendations> _logger;
    private readonly IChildBookRepository _repository;
    private readonly INotificationService _notificationService;

    public SaveUsersRecommendations(
        ILogger<SaveUsersRecommendations> logger,
        IChildBookRepository repository,
        INotificationService notificationService)
    {
        _logger = logger;
        _repository = repository;
        _notificationService = notificationService;
    }

    public async Task Run()
    {
        _logger.LogInformation("Job {JobName} started", nameof(SaveUsersRecommendations));

        var childBooks = await _repository.GetAll();

        foreach (var childBook in childBooks)
        {
            var countBefore = childBook.Recommendations.Count;

            foreach (var answer in childBook.Child.Answers.Where(answer => !answer.IsCorrect()))
            {
                childBook.AddRecommendationsIfNeeded(answer.Question.Recommendations);
            }

            if (childBook.Recommendations.Count != countBefore)
            {
                await _repository.Update(childBook);
                await _notificationService.CreateIfNotExists(childBook.Id, NotificationType.NewRecommendations);
            }
        }

        _logger.LogInformation("Job {JobName} completed", nameof(SaveUsersRecommendations));
    }
}