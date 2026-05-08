using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Application.Resources;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Exceptions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Services;

public class ChildService : IChildService
{
    private readonly IChildRepository _childRepository;
    private readonly IUserService _userService;
    private readonly ILogger<ChildService> _logger;
    private readonly IStringLocalizer<AppMessages> _localizer;

    public ChildService(IChildRepository childRepository, IUserService userService, ILogger<ChildService> logger, IStringLocalizer<AppMessages> localizer)
    {
        _childRepository = childRepository;
        _userService = userService;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task Create(CreateChildDto dto)
    {
        _ = await _userService.GetAndValidateById(dto.PrcId);
        
        await _childRepository.Create(dto.ToDomainObject());
    }

    public async Task<List<ChildDto>> GetAllById(Guid prcId)
    {
        var children = await _childRepository.GetAllById(prcId);

        var childrenDto = new List<ChildDto>();

        children.ForEach(x => childrenDto.Add(new ChildDto(x)));

        return childrenDto;
    }
    
    public async Task<Child> GetAndValidateById(Guid id, Guid prcId)
    {
        var child = await _childRepository.GetById(id);

        if (child is null)
        {
            _logger.LogWarning("Child {ChildId} not found", id);
            throw new NotFoundException(_localizer["ChildNotFound"]);
        }
        
        if (child.PrcId != prcId)
        {
            _logger.LogWarning("Child {ChildId} does not belong to user {UserId}", id, prcId);
            throw new ValidationException(_localizer["ChildNotBelongsToUser"]);
        }

        return child;
    }

    public async Task Update(Guid prcId, Guid childId, UpdateChildDto dto)
    {
        _ = await _userService.GetAndValidateById(prcId);
        
        var child = await GetAndValidateById(childId, prcId);

        child.Update(dto.Name, dto.BirthDate, dto.Sex, dto.MonthsOfBreastFeeding, dto.IsPremature);
        
        await _childRepository.Update(child);
    }
}