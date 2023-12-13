using CRIApp.Application.Abstractions;
using CRIApp.Application.Dto;
using CRIApp.Domain.Data;
using CRIApp.Domain.Exceptions;

namespace CRIApp.Application.Services;

public class AnswerService : IAnswerService
{
    private readonly IAnswerRepository _repository;
    private readonly IQuestionService _questionService;
    private readonly IUserService _userService;

    public AnswerService(IAnswerRepository repository, IQuestionService questionService, IUserService userService)
    {
        _repository = repository;
        _questionService = questionService;
        _userService = userService;
    }

    public async Task Create(CreateAnswerDto dto)
    {
        _ = await _questionService.GetAndValidateById(dto.QuestionId);
        var user = await _userService.GetAndValidateById(dto.PrcId);
        var child = user!.Children.FirstOrDefault(x => x.Id == dto.ChildId);

        if (child is null)
            throw new NotFoundException($"User {dto.PrcId} does not have a child with id {dto.ChildId}");
        
        await _repository.Create(dto.ToDomainObject());
    }
}