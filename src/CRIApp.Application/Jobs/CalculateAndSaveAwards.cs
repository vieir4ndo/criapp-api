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
        var awards = await _awardRepository.GetAll();
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);
        var dayBefore = today.AddDays(-2);

        foreach (var childBook in childBooks)
        {
            var answers = childBook.Child.Answers;
            var scoreYesterday = CalculateScore(answers.Where(a => a.CreatedAt.Date <= yesterday).ToList());
            var scoreDayBefore = CalculateScore(answers.Where(a => a.CreatedAt.Date <= dayBefore).ToList());

            var countBefore = childBook.Awards.Count;

            EvaluateFirstSteps(childBook, answers, awards);
            EvaluateEngagedFamily(childBook, answers, awards);
            EvaluateOvercoming(childBook, scoreYesterday, scoreDayBefore, awards);
            EvaluatePositiveEnvironment(childBook, awards);
            EvaluateSmallProgress(childBook, scoreYesterday, scoreDayBefore, awards);

            if (childBook.Awards.Count > countBefore)
                await _childBookRepository.Update(childBook);
        }

        _logger.LogInformation("Job {JobName} completed", nameof(CalculateAndSaveAwards));
    }

    private static void EvaluateFirstSteps(ChildBook childBook, List<Answer> answers, IList<Award> awards)
    {
        var distinctQuestionsAnswered = answers.Select(a => a.QuestionId).Distinct().Count();
        if (distinctQuestionsAnswered >= 5)
        {
            var award = awards.FirstOrDefault(a => a.Type == AwardType.FirstSteps);
            if (award != null) childBook.AddAwardIfNeeded(award);
        }
    }

    private static void EvaluateEngagedFamily(ChildBook childBook, List<Answer> answers, IList<Award> awards)
    {
        var answerDates = answers
            .Select(a => a.CreatedAt.Date)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        if (HasThreeConsecutiveDays(answerDates))
        {
            var award = awards.FirstOrDefault(a => a.Type == AwardType.EngagedFamily);
            if (award != null) childBook.AddAwardIfNeeded(award);
        }
    }

    private static void EvaluateOvercoming(ChildBook childBook, int scoreYesterday, int scoreDayBefore, IList<Award> awards)
    {
        if (scoreDayBefore > 0 && scoreYesterday >= scoreDayBefore * 1.20)
        {
            var award = awards.FirstOrDefault(a => a.Type == AwardType.Overcoming);
            if (award != null) childBook.AddAwardIfNeeded(award);
        }
    }

    private static void EvaluatePositiveEnvironment(ChildBook childBook, IList<Award> awards)
    {
        if (childBook.Score > 80)
        {
            var award = awards.FirstOrDefault(a => a.Type == AwardType.PositiveEnvironment);
            if (award != null) childBook.AddAwardIfNeeded(award);
        }
    }

    private static void EvaluateSmallProgress(ChildBook childBook, int scoreYesterday, int scoreDayBefore, IList<Award> awards)
    {
        if (scoreYesterday > scoreDayBefore)
        {
            var award = awards.FirstOrDefault(a => a.Type == AwardType.SmallProgress);
            if (award != null) childBook.AddAwardIfNeeded(award);
        }
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