using CRIApp.Application.Abstractions;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

public class AwardChildBookJob : IRecurrentJob
{
    private readonly ILogger<AwardChildBookJob> _logger;
    private readonly IChildBookRepository _childBookRepository;
    private readonly IAwardRepository _awardRepository;
    private readonly INotificationService _notificationService;
    private readonly IQuestionService _questionService;

    public AwardChildBookJob(
        ILogger<AwardChildBookJob> logger,
        IChildBookRepository childBookRepository,
        IAwardRepository awardRepository,
        INotificationService notificationService,
        IQuestionService questionService)
    {
        _logger = logger;
        _childBookRepository = childBookRepository;
        _awardRepository = awardRepository;
        _notificationService = notificationService;
        _questionService = questionService;
    }

    public async Task Run()
    {
        _logger.LogInformation("Job {JobName} started", nameof(AwardChildBookJob));

        var childBooks = await _childBookRepository.GetAll();
        var awards = await _awardRepository.GetAll();

        var questionsByGroup = new Dictionary<QuestionGroup, int>();
        foreach (QuestionGroup group in Enum.GetValues(typeof(QuestionGroup)))
        {
            var questions = await _questionService.GetByGroup(group);
            questionsByGroup[group] = questions.Count();
        }

        foreach (var childBook in childBooks)
        {
            var awardCountBefore = childBook.Awards.Count;
            var answers = childBook.Child.Answers;

            EvaluateHabitBuilder(childBook, awards);
            EvaluateConsistentParent(childBook, awards);
            EvaluateSilverMedal(childBook, answers, QuestionGroup.CognitiveDevelopment, AwardType.SilverCognitiveDevelopment, awards, questionsByGroup[QuestionGroup.CognitiveDevelopment]);
            EvaluateSilverMedal(childBook, answers, QuestionGroup.EmotionalBonding, AwardType.SilverEmotionalBonding, awards, questionsByGroup[QuestionGroup.EmotionalBonding]);
            EvaluateSilverMedal(childBook, answers, QuestionGroup.DailyRoutines, AwardType.SilverDailyRoutines, awards, questionsByGroup[QuestionGroup.DailyRoutines]);
            EvaluateSilverMedal(childBook, answers, QuestionGroup.SocialInteraction, AwardType.SilverSocialInteraction, awards, questionsByGroup[QuestionGroup.SocialInteraction]);
            EvaluateGoldMedal(childBook, awards);

            await _childBookRepository.Update(childBook);

            if (childBook.Awards.Count > awardCountBefore)
                await _notificationService.CreateIfNotExists(childBook.Id, NotificationType.NewAward);
        }

        _logger.LogInformation("Job {JobName} completed", nameof(AwardChildBookJob));
    }

    private static void EvaluateHabitBuilder(ChildBook childBook, IList<Award> awards)
    {
        var completedDates = childBook.ChildBookRecommendations.SelectMany(cbr => cbr.Checkpoints)
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
            if (award is not null) childBook.AddAwardIfNeeded(award);
        }
    }

    private static void EvaluateConsistentParent(ChildBook childBook, IList<Award> awards)
    {
        var distinctDays = childBook.ChildBookRecommendations.SelectMany(cbr => cbr.Checkpoints)
            .Where(c => c.Completed)
            .Select(c => c.Date)
            .Distinct()
            .Count();

        if (distinctDays >= 7)
        {
            var award = awards.FirstOrDefault(a => a.Type == AwardType.ConsistentParent);
            if (award is not null) childBook.AddAwardIfNeeded(award);
        }
    }

    private static void EvaluateSilverMedal(ChildBook childBook, IList<Answer> answers, QuestionGroup group, AwardType awardType, IList<Award> awards, int totalQuestionsInGroup)
    {
        var groupAnswers = answers.Where(a => a.Question.Group == group).ToList();

        if (totalQuestionsInGroup == 0 || groupAnswers.Count < totalQuestionsInGroup) return;

        if (CalculateGroupScore(groupAnswers) == 100)
        {
            var award = awards.FirstOrDefault(a => a.Type == awardType);
            if (award is not null) childBook.AddAwardIfNeeded(award);
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
            if (award is not null) childBook.AddAwardIfNeeded(award);
        }
    }

    private static int CalculateGroupScore(List<Answer> groupAnswers)
    {
        var points = groupAnswers.Count(a => a.IsCorrect());

        return points * 100 / groupAnswers.Count;
    }
}
