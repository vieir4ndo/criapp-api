using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Data;
using CRIApp.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Services;

public class AnswerService : IAnswerService
{
    private readonly IAnswerRepository _repository;
    private readonly IQuestionService _questionService;
    private readonly IUserService _userService;
    private readonly ILogger<AnswerService> _logger;

    public AnswerService(IAnswerRepository repository, IQuestionService questionService, IUserService userService, ILogger<AnswerService> logger)
    {
        _repository = repository;
        _questionService = questionService;
        _userService = userService;
        _logger = logger;
    }

    public async Task Create(CreateAnswerDto dto)
    {
        _ = await _questionService.GetAndValidateById(dto.QuestionId);
        var user = await _userService.GetAndValidateById(dto.PrcId);
        var child = user!.Children.FirstOrDefault(x => x.Id == dto.ChildId);

        if (child is null)
        {
            _logger.LogWarning("User {UserId} attempted to answer for unknown child {ChildId}", dto.PrcId, dto.ChildId);
            throw new NotFoundException($"Usuario {dto.PrcId} no tiene niño(a) con id {dto.ChildId}");
        }

        try
        {
            await _repository.Create(dto.ToDomainObject());
        }
        finally
        {
            await _userService.UpdateScore(user);
        }
    }
}