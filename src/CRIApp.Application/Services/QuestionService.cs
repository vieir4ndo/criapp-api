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
    public async Task<IEnumerable<QuestionDto>> GetAllQuestionsToReply(Guid userId)
    {
        var user = await _userService.GetAndValidateById(userId);
        
        var questions = await _questionRepository.GetAllExcept(user!.Answers.Select(answer => answer.QuestionId).ToList());

        return questions.Select(question => new QuestionDto(question));
    }

    public async Task<Question?> GetAndValidateById(Guid id)
    {
        var question = await _questionRepository.GetById(id);

        if (question is null)
            throw new NotFoundException("Question not found");

        return question;
    }
}