using CRIApp.Domain.Data;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

public class SaveUsersAdvices : IRecurrentJob
{
    private readonly ILogger<SaveUsersAdvices> _logger;
    private readonly IUserRepository _repository;

    public SaveUsersAdvices(ILogger<SaveUsersAdvices> logger, IUserRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task Run()
    {
        _logger.LogInformation("Job {JobName} started", nameof(SaveUsersAdvices));

        var users = await _repository.GetAll();

        foreach (var user in users)
        {
            var countBefore = user.Advices.Count;

            foreach (var answer in user.Answers)
            {
                List<int> values = answer.Question.ScoringType switch
                {
                    QuestionScoringType.Default => [1, 2, 3, 4],
                    QuestionScoringType.Reversed => [5, 6],
                    _ => throw new ArgumentOutOfRangeException()
                };

                if (!values.Contains(answer.Value))
                {
                    user.AddAdvicesIfNeeded(answer.Question.Advices);
                }
            }

            if (user.Advices.Count != countBefore)
            {
                await _repository.Update(user);
            }
        }

        _logger.LogInformation("Job {JobName} completed", nameof(SaveUsersAdvices));
    }
}