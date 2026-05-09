using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Application.Resources;
using CRIApp.Domain.Data;
using CRIApp.Domain.Exceptions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Services;

public class AnswerService : IAnswerService
{
    private readonly IAnswerRepository _repository;
    private readonly IQuestionService _questionService;
    private readonly IUserService _userService;
    private readonly ILogger<AnswerService> _logger;
    private readonly IStringLocalizer<AppMessages> _localizer;

    public AnswerService(IAnswerRepository repository, IQuestionService questionService, IUserService userService, ILogger<AnswerService> logger, IStringLocalizer<AppMessages> localizer)
    {
        _repository = repository;
        _questionService = questionService;
        _userService = userService;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task Create(CreateAnswerDto dto)
    {
        _ = await _questionService.GetAndValidateById(dto.QuestionId);
        var user = await _userService.GetAndValidateById(dto.PrcId);
        var child = user!.Children.FirstOrDefault(x => x.Id == dto.ChildId);

        if (child is null)
        {
            _logger.LogWarning("User {UserId} attempted to answer for unknown child {ChildId}", dto.PrcId, dto.ChildId);
            throw new NotFoundException(_localizer["ChildNotFoundForUser", dto.PrcId, dto.ChildId]);
        }

        await _repository.Create(dto.ToDomainObject());
    }
}