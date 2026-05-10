using CRIApp.Domain.Data;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

public class SaveUsersRecommendations : IRecurrentJob
{
    private readonly ILogger<SaveUsersRecommendations> _logger;
    private readonly IChildBookRepository _repository;

    public SaveUsersRecommendations(ILogger<SaveUsersRecommendations> logger, IChildBookRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task Run()
    {
        _logger.LogInformation("Job {JobName} started", nameof(SaveUsersRecommendations));

        var childBooks = await _repository.GetAll();

        foreach (var childBook in childBooks)
        {
            var countBefore = childBook.Recommendations.Count;

            foreach (var answer in childBook.Child.Answers)
            {
                List<int> values = answer.Question.ScoringType switch
                {
                    QuestionScoringType.Default => [1, 2, 3, 4],
                    QuestionScoringType.Reversed => [5, 6],
                    _ => throw new ArgumentOutOfRangeException()
                };

                if (!values.Contains(answer.Value))
                {
                    childBook.AddRecommendationsIfNeeded(answer.Question.Recommendations);
                }
            }

            if (childBook.Recommendations.Count != countBefore)
            {
                await _repository.Update(childBook);
            }
        }

        _logger.LogInformation("Job {JobName} completed", nameof(SaveUsersRecommendations));
    }
}