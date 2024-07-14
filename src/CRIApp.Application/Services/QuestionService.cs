using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Exceptions;

namespace CRIApp.Application.Services;

public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IUserService _userService;

    public QuestionService(IQuestionRepository questionRepository, IUserService userService)
    {
        _questionRepository = questionRepository;
        _userService = userService;
    }
    public async Task<IEnumerable<QuestionDto>> GetQuestionsToReply(Guid userId, Guid childId)
    {
        var user = await _userService.GetAndValidateById(userId);

        var answeredQuestions = user.Answers.Where(x => x.ChildId == childId).Select(answer => answer.QuestionId).ToList();
        
        var questions = await _questionRepository.GetAllExcept(answeredQuestions);
        
        var questionsAnsweredToday = user.Answers.Count(x =>  x.ChildId == childId && x.CreatedAt.Date == DateTime.Today);

        return questions.Select(question => new QuestionDto(question)).Take(10 - questionsAnsweredToday);
    }

    public async Task<Question?> GetAndValidateById(Guid id)
    {
        var question = await _questionRepository.GetById(id);

        if (question is null)
            throw new NotFoundException("Pregunta no encontrada");

        return question;
    }
}