using CRIApp.Domain.Data;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

public class UpdateChildBookScoreJob : IRecurrentJob
{
    private readonly ILogger<UpdateChildBookScoreJob> _logger;
    private readonly IChildBookRepository _childBookRepository;

    public UpdateChildBookScoreJob(
        ILogger<UpdateChildBookScoreJob> logger,
        IChildBookRepository childBookRepository)
    {
        _logger = logger;
        _childBookRepository = childBookRepository;
    }

    public async Task Run()
    {
        _logger.LogInformation("Job {JobName} started", nameof(UpdateChildBookScoreJob));

        var childBooks = await _childBookRepository.GetAll();

        foreach (var childBook in childBooks)
        {
            childBook.UpdateScore(childBook.Child.Answers);
            await _childBookRepository.Update(childBook);
        }

        _logger.LogInformation("Job {JobName} completed", nameof(UpdateChildBookScoreJob));
    }
}
