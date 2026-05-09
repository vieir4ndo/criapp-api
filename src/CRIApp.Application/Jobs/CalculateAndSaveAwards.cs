using CRIApp.Application.Abstractions;
using CRIApp.Domain.Data;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

public class CalculateAndSaveAwards : IRecurrentJob
{
    private readonly ILogger<CalculateAndSaveAwards> _logger;
    private readonly IUserRepository _userRepository;

    public CalculateAndSaveAwards(ILogger<CalculateAndSaveAwards> logger, IUserRepository userRepository, IUserService userService)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task Run()
    {
        _logger.LogInformation("Job {JobName} started", nameof(CalculateAndSaveAwards));

        var users = await _userRepository.GetAll();
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);
        var dayBefore = today.AddDays(-2);

        foreach (var user in users)
        {
            if (user.IsAdmin) continue;

            var scoreYesterday = CalculateScore(user.Answers.Where(a => a.CreatedAt.Date <= yesterday).ToList());
            var scoreDayBefore = CalculateScore(user.Answers.Where(a => a.CreatedAt.Date <= dayBefore).ToList());

            EvaluateFirstSteps(user);
            EvaluateEngagedFamily(user);
            EvaluateOvercoming(user, scoreYesterday, scoreDayBefore);
            EvaluatePositiveEnvironment(user);
            EvaluateSmallProgress(user, scoreYesterday, scoreDayBefore);

            await _userRepository.Update(user);
        }

        _logger.LogInformation("Job {JobName} completed", nameof(CalculateAndSaveAwards));
    }

    private static void EvaluateFirstSteps(Domain.Entities.User user)
    {
        var distinctQuestionsAnswered = user.Answers.Select(a => a.QuestionId).Distinct().Count();
        if (distinctQuestionsAnswered >= 5)
            user.AddAwardIfNeeded(AwardType.FirstSteps);
    }

    private static void EvaluateEngagedFamily(Domain.Entities.User user)
    {
        var answerDates = user.Answers
            .Select(a => a.CreatedAt.Date)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        if (HasThreeConsecutiveDays(answerDates))
            user.AddAwardIfNeeded(AwardType.EngagedFamily);
    }

    private static void EvaluateOvercoming(Domain.Entities.User user, int scoreYesterday, int scoreDayBefore)
    {
        if (scoreDayBefore > 0 && scoreYesterday >= scoreDayBefore * 1.20)
            user.AddAwardIfNeeded(AwardType.Overcoming);
    }

    private static void EvaluatePositiveEnvironment(Domain.Entities.User user)
    {
        if (user.Score > 80)
            user.AddAwardIfNeeded(AwardType.PositiveEnvironment);
    }

    private static void EvaluateSmallProgress(Domain.Entities.User user, int scoreYesterday, int scoreDayBefore)
    {
        if (scoreYesterday > scoreDayBefore)
            user.AddAwardIfNeeded(AwardType.SmallProgress);
    }

    private static bool HasThreeConsecutiveDays(List<DateTime> sortedDates)
    {
        for (var i = 0; i <= sortedDates.Count - 3; i++)
        {
            if (sortedDates[i + 1] == sortedDates[i].AddDays(1) &&
                sortedDates[i + 2] == sortedDates[i].AddDays(2))
                return true;
        }
        return false;
    }

    private static int CalculateScore(List<Domain.Entities.Answer> answers)
    {
        if (answers.Count == 0) return 0;

        var points = answers.Count(a =>
        {
            var valid = a.Question.ScoringType switch
            {
                QuestionScoringType.Default => new[] { 1, 2, 3, 4 },
                QuestionScoringType.Reversed => new[] { 5, 6 },
                _ => Array.Empty<int>(),
            };
            return valid.Contains(a.Value);
        });

        return points * 100 / answers.Count;
    }
}