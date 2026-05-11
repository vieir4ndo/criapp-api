using System.Linq.Expressions;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Application.Resources;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Services;

public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IUserService _userService;
    private readonly ILogger<QuestionService> _logger;
    private readonly IStringLocalizer<AppMessages> _localizer;

    public QuestionService(IQuestionRepository questionRepository, IUserService userService,
        ILogger<QuestionService> logger, IStringLocalizer<AppMessages> localizer)
    {
        _questionRepository = questionRepository;
        _userService = userService;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<IEnumerable<QuestionDto>> GetQuestionsToReply(Guid userId, Guid childId)
    {
        var user = await _userService.GetAndValidateById(userId);

        var remaining = GetRemainingQuestions(user, childId);

        if (remaining <= 0)
            return Enumerable.Empty<QuestionDto>();

        var recentIds = GetRecentlyAnsweredQuestionIds(user, childId);

        var preferred = await GetAvailableQuestions(user.FamilyType, recentIds);
        var fallback = await GetFallbackPool(user.FamilyType, preferred);

        return SelectByGroup(preferred, fallback, remaining);
    }

    private int GetRemainingQuestions(User user, Guid childId)
    {
        var today = DateTime.UtcNow.Date;

        var answeredToday = user.Answers.Count(x =>
            x.ChildId == childId &&
            x.CreatedAt.Date == today);

        return 5 - answeredToday;
    }

    private List<Guid> GetRecentlyAnsweredQuestionIds(User user, Guid childId)
    {
        var today = DateTime.UtcNow.Date;

        return user.Answers
            .Where(x =>
                x.ChildId == childId &&
                x.CreatedAt >= today.AddDays(-7))
            .Select(x => x.QuestionId)
            .Distinct()
            .ToList();
    }

    private async Task<List<Question>> GetAvailableQuestions(
        FamilyType? familyType,
        List<Guid> recentQuestionIds)
    {
        Expression<Func<Question, bool>> filter = familyType.HasValue
            ? q => q.Active && !recentQuestionIds.Contains(q.Id) && q.ExcludedFamilyTypes.All(f => f != familyType.Value)
            : q => q.Active && !recentQuestionIds.Contains(q.Id);

        return await _questionRepository.GetFiltered(filter);
    }

    private async Task<List<Question>> GetFallbackPool(FamilyType? familyType, List<Question> preferred)
    {
        var preferredIds = preferred.Select(q => q.Id).ToList();

        Expression<Func<Question, bool>> filter = familyType.HasValue
            ? q => q.Active && !preferredIds.Contains(q.Id) && q.ExcludedFamilyTypes.All(f => f != familyType.Value)
            : q => q.Active && !preferredIds.Contains(q.Id);

        return await _questionRepository.GetFiltered(filter);
    }

    private IEnumerable<QuestionDto> SelectByGroup(
        List<Question> preferred,
        List<Question> fallback,
        int limit)
    {
        var selected = new List<Question>();
        var preferredIds = preferred.Select(q => q.Id).ToHashSet();

        var byGroup = preferred
            .Concat(fallback)
            .DistinctBy(q => q.Id)
            .GroupBy(q => q.Group)
            .ToList();

        foreach (var group in byGroup)
        {
            if (selected.Count >= limit) break;

            var nonRecent = group.Where(q => preferredIds.Contains(q.Id)).ToList();
            var candidate = nonRecent.Count > 0
                ? nonRecent.OrderBy(_ => Guid.NewGuid()).First()
                : group.OrderBy(_ => Guid.NewGuid()).First();

            selected.Add(candidate);
        }

        if (selected.Count < limit)
        {
            var selectedIds = selected.Select(q => q.Id).ToHashSet();

            var extras = preferred
                .Concat(fallback)
                .DistinctBy(q => q.Id)
                .Where(q => !selectedIds.Contains(q.Id))
                .OrderBy(_ => Guid.NewGuid())
                .Take(limit - selected.Count);

            selected.AddRange(extras);
        }

        return selected
            .OrderBy(_ => Guid.NewGuid())
            .Select(q => new QuestionDto(q));
    }

    public async Task<Question?> GetAndValidateById(Guid id)
    {
        var question = await _questionRepository.GetById(id);

        if (question is null)
        {
            _logger.LogWarning("Question {QuestionId} not found", id);
            throw new NotFoundException(_localizer["QuestionNotFound"]);
        }

        return question;
    }

    public async Task<IEnumerable<Question>> GetByGroup(QuestionGroup group)
    {
        return await _questionRepository.GetFiltered(q => q.Active && q.Group == group);
    }
}
