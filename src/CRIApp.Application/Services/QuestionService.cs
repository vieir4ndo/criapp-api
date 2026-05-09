using System.Linq.Expressions;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Services;

public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IUserService _userService;
    private readonly ILogger<QuestionService> _logger;

    public QuestionService(IQuestionRepository questionRepository, IUserService userService, ILogger<QuestionService> logger)
    {
        _questionRepository = questionRepository;
        _userService = userService;
        _logger = logger;
    }
    public async Task<IEnumerable<QuestionDto>> GetQuestionsToReply(Guid userId, Guid childId)
    {
        var user = await _userService.GetAndValidateById(userId);

        var answeredQuestions = user.Answers
            .Where(x => x.ChildId == childId)
            .Select(x => x.QuestionId)
            .ToList();

        var questionsAnsweredToday = user.Answers
            .Count(x => x.ChildId == childId &&
                        x.CreatedAt.Date == DateTime.Today);

        var familyType = user.FamilyType!.Value;

        Expression<Func<Question, bool>> filter = q =>
            q.Active &&
            !answeredQuestions.Contains(q.Id) && 
            q.ExcludedFamilyTypes.All(f => f != familyType);

        var questions = await _questionRepository.GetFiltered(filter);

        return questions
            .Take(10 - questionsAnsweredToday)
            .Select(q => new QuestionDto(q));
    }

    public async Task<Question?> GetAndValidateById(Guid id)
    {
        var question = await _questionRepository.GetById(id);

        if (question is null)
        {
            _logger.LogWarning("Question {QuestionId} not found", id);
            throw new NotFoundException("Pregunta no encontrada");
        }

        return question;
    }
}