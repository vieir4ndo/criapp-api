using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

public class CalculateAndSaveAwards : IRecurrentJob
{
    private readonly ILogger<CalculateAndSaveAwards> _logger;
    private readonly IChildBookRepository _childBookRepository;
    private readonly IAwardRepository _awardRepository;

    public CalculateAndSaveAwards(ILogger<CalculateAndSaveAwards> logger, IChildBookRepository childBookRepository, IAwardRepository awardRepository)
    {
        _logger = logger;
        _childBookRepository = childBookRepository;
        _awardRepository = awardRepository;
    }

    public async Task Run()
    {
        _logger.LogInformation("Job {JobName} started", nameof(CalculateAndSaveAwards));

        var childBooks = await _childBookRepository.GetAll();
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);
        var dayBefore = today.AddDays(-2);

        foreach (var childBook in childBooks)
        {
            var answers = childBook.Child.Answers;
            var scoreYesterday = CalculateScore(answers.Where(a => a.CreatedAt.Date <= yesterday).ToList());
            var scoreDayBefore = CalculateScore(answers.Where(a => a.CreatedAt.Date <= dayBefore).ToList());

            var awardsBefore = childBook.Awards.Select(a => a.Id).ToHashSet();

            EvaluateFirstSteps(childBook, answers);
            EvaluateEngagedFamily(childBook, answers);
            EvaluateOvercoming(childBook, scoreYesterday, scoreDayBefore);
            EvaluatePositiveEnvironment(childBook);
            EvaluateSmallProgress(childBook, scoreYesterday, scoreDayBefore);

            var newAwards = childBook.Awards.Where(a => !awardsBefore.Contains(a.Id)).ToList();
            if (newAwards.Count > 0)
                await _awardRepository.CreateRange(newAwards);
        }

        _logger.LogInformation("Job {JobName} completed", nameof(CalculateAndSaveAwards));
    }

    private static void EvaluateFirstSteps(ChildBook childBook, List<Answer> answers)
    {
        var distinctQuestionsAnswered = answers.Select(a => a.QuestionId).Distinct().Count();
        if (distinctQuestionsAnswered >= 5)
            childBook.AddAwardIfNeeded(AwardType.FirstSteps);
    }

    private static void EvaluateEngagedFamily(ChildBook childBook, List<Answer> answers)
    {
        var answerDates = answers
            .Select(a => a.CreatedAt.Date)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        if (HasThreeConsecutiveDays(answerDates))
            childBook.AddAwardIfNeeded(AwardType.EngagedFamily);
    }

    private static void EvaluateOvercoming(ChildBook childBook, int scoreYesterday, int scoreDayBefore)
    {
        if (scoreDayBefore > 0 && scoreYesterday >= scoreDayBefore * 1.20)
            childBook.AddAwardIfNeeded(AwardType.Overcoming);
    }

    private static void EvaluatePositiveEnvironment(ChildBook childBook)
    {
        if (childBook.Score > 80)
            childBook.AddAwardIfNeeded(AwardType.PositiveEnvironment);
    }

    private static void EvaluateSmallProgress(ChildBook childBook, int scoreYesterday, int scoreDayBefore)
    {
        if (scoreYesterday > scoreDayBefore)
            childBook.AddAwardIfNeeded(AwardType.SmallProgress);
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

    private static int CalculateScore(List<Answer> answers)
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