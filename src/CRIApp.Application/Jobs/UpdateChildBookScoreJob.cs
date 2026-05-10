using CRIApp.Domain.Data;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

public class UpdateChildBookScoreJob : IRecurrentJob
{
    private readonly ILogger<UpdateChildBookScoreJob> _logger;
    private readonly IChildBookRepository _childBookRepository;
    private readonly IRecommendationCheckpointRepository _checkpointRepository;

    public UpdateChildBookScoreJob(
        ILogger<UpdateChildBookScoreJob> logger,
        IChildBookRepository childBookRepository,
        IRecommendationCheckpointRepository checkpointRepository)
    {
        _logger = logger;
        _childBookRepository = childBookRepository;
        _checkpointRepository = checkpointRepository;
    }

    public async Task Run()
    {
        _logger.LogInformation("Job {JobName} started", nameof(UpdateChildBookScoreJob));

        var today = DateTime.UtcNow.Date;
        var childBooks = await _childBookRepository.GetAll();

        foreach (var childBook in childBooks)
        {
            var todayCheckpoints = await _checkpointRepository.GetByChildBookIdAndDate(childBook.Id, today);
            childBook.UpdateScore(childBook.Child.Answers, todayCheckpoints.ToList());
            await _childBookRepository.Update(childBook);
        }

        _logger.LogInformation("Job {JobName} completed", nameof(UpdateChildBookScoreJob));
    }
}