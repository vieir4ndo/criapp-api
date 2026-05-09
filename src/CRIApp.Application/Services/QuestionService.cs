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

        var remainingQuestions = GetRemainingQuestions(user, childId);

        if (remainingQuestions <= 0)
            return Enumerable.Empty<QuestionDto>();

        var recentQuestionIds = GetRecentlyAnsweredQuestionIds(user, childId);

        var questions = await GetAvailableQuestions(user.FamilyType, recentQuestionIds);

        questions = await ApplyFallbackIfNeeded(
            questions,
            user.FamilyType,
            remainingQuestions);

        return GetRandomQuestions(questions, remainingQuestions);
    }

    private int GetRemainingQuestions(User user, Guid childId)
    {
        var today = DateTime.Today;

        var answeredToday = user.Answers.Count(x =>
            x.ChildId == childId &&
            x.CreatedAt.Date == today);

        return 5 - answeredToday;
    }

    private List<Guid> GetRecentlyAnsweredQuestionIds(User user, Guid childId)
    {
        var today = DateTime.Today;

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

    private async Task<List<Question>> ApplyFallbackIfNeeded(
        List<Question> questions,
        FamilyType? familyType,
        int remainingQuestions)
    {
        if (questions.Count >= remainingQuestions)
            return questions;

        var currentIds = questions
            .Select(q => q.Id)
            .ToList();

        Expression<Func<Question, bool>> fallbackFilter = familyType.HasValue
            ? q => q.Active && !currentIds.Contains(q.Id) && q.ExcludedFamilyTypes.All(f => f != familyType.Value)
            : q => q.Active && !currentIds.Contains(q.Id);

        var fallbackQuestions = await _questionRepository.GetFiltered(fallbackFilter);

        questions.AddRange(fallbackQuestions);

        return questions;
    }

    private IEnumerable<QuestionDto> GetRandomQuestions(
        List<Question> questions,
        int remainingQuestions)
    {
        return questions
            .DistinctBy(q => q.Id)
            .OrderBy(_ => Guid.NewGuid())
            .Take(remainingQuestions)
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
}