using CRIApp.Application.Abstractions;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

public class UpdateAndAwardChildBookJob : IRecurrentJob
{
    private readonly ILogger<UpdateAndAwardChildBookJob> _logger;
    private readonly IChildBookRepository _childBookRepository;
    private readonly IRecommendationCheckpointRepository _checkpointRepository;
    private readonly IAwardRepository _awardRepository;
    private readonly INotificationService _notificationService;

    public UpdateAndAwardChildBookJob(
        ILogger<UpdateAndAwardChildBookJob> logger,
        IChildBookRepository childBookRepository,
        IRecommendationCheckpointRepository checkpointRepository,
        IAwardRepository awardRepository,
        INotificationService notificationService)
    {
        _logger = logger;
        _childBookRepository = childBookRepository;
        _checkpointRepository = checkpointRepository;
        _awardRepository = awardRepository;
        _notificationService = notificationService;
    }

    public async Task Run()
    {
        _logger.LogInformation("Job {JobName} started", nameof(UpdateAndAwardChildBookJob));

        var today = DateTime.UtcNow.Date;
        var childBooks = await _childBookRepository.GetAll();
        var awards = await _awardRepository.GetAll();

        foreach (var childBook in childBooks)
        {
            var todayCheckpoints = await _checkpointRepository.GetByChildBookIdAndDate(childBook.Id, today);
            childBook.UpdateScore(childBook.Child.Answers, todayCheckpoints.ToList());

            var awardCountBefore = childBook.Awards.Count;
            var answers = childBook.Child.Answers;

            EvaluateHabitBuilder(childBook, awards);
            EvaluateConsistentParent(childBook, awards);
            EvaluateSilverMedal(childBook, answers, QuestionGroup.CognitiveDevelopment, AwardType.SilverCognitiveDevelopment, awards);
            EvaluateSilverMedal(childBook, answers, QuestionGroup.EmotionalBonding, AwardType.SilverEmotionalBonding, awards);
            EvaluateSilverMedal(childBook, answers, QuestionGroup.DailyRoutines, AwardType.SilverDailyRoutines, awards);
            EvaluateSilverMedal(childBook, answers, QuestionGroup.SocialInteraction, AwardType.SilverSocialInteraction, awards);
            EvaluateGoldMedal(childBook, awards);

            await _childBookRepository.Update(childBook);

            if (childBook.Awards.Count > awardCountBefore)
                await _notificationService.CreateIfNotExists(childBook.Id, NotificationType.NewAward);
        }

        _logger.LogInformation("Job {JobName} completed", nameof(UpdateAndAwardChildBookJob));
    }

    private static void EvaluateHabitBuilder(ChildBook childBook, IList<Award> awards)
    {
        var completedDates = childBook.RecommendationCheckpoints
            .Where(c => c.Completed)
            .Select(c => c.Date)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        var qualifies = completedDates
            .Any(d => completedDates.Count(cd => cd >= d && cd <= d.AddDays(6)) >= 4);

        if (qualifies)
        {
            var award = awards.FirstOrDefault(a => a.Type == AwardType.HabitBuilder);
            if (award != null) childBook.AddAwardIfNeeded(award);
        }
    }

    private static void EvaluateConsistentParent(ChildBook childBook, IList<Award> awards)
    {
        var distinctDays = childBook.RecommendationCheckpoints
            .Where(c => c.Completed)
            .Select(c => c.Date)
            .Distinct()
            .Count();

        if (distinctDays >= 7)
        {
            var award = awards.FirstOrDefault(a => a.Type == AwardType.ConsistentParent);
            if (award != null) childBook.AddAwardIfNeeded(award);
        }
    }

    private static void EvaluateSilverMedal(ChildBook childBook, IList<Answer> answers, QuestionGroup group, AwardType awardType, IList<Award> awards)
    {
        var groupAnswers = answers.Where(a => a.Question.Group == group).ToList();
        if (!groupAnswers.Any()) return;

        if (CalculateGroupScore(groupAnswers) == 100)
        {
            var award = awards.FirstOrDefault(a => a.Type == awardType);
            if (award != null) childBook.AddAwardIfNeeded(award);
        }
    }

    private static void EvaluateGoldMedal(ChildBook childBook, IList<Award> awards)
    {
        var silverTypes = new[]
        {
            AwardType.SilverCognitiveDevelopment,
            AwardType.SilverEmotionalBonding,
            AwardType.SilverDailyRoutines,
            AwardType.SilverSocialInteraction,
        };

        if (silverTypes.All(t => childBook.Awards.Any(a => a.Type == t)))
        {
            var award = awards.FirstOrDefault(a => a.Type == AwardType.GoldParent);
            if (award != null) childBook.AddAwardIfNeeded(award);
        }
    }

    private static int CalculateGroupScore(List<Answer> groupAnswers)
    {
        var points = groupAnswers.Count(a =>
        {
            var valid = a.Question.ScoringType switch
            {
                QuestionScoringType.Default => new[] { 5, 6 },
                QuestionScoringType.Reversed => new[] { 1, 2 },
                _ => Array.Empty<int>(),
            };
            return valid.Contains(a.Value);
        });

        return points * 100 / groupAnswers.Count;
    }
}
