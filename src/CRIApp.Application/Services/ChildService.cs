using CRIApp.Application.Abstractions;
using CRIApp.Application.Dto;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Exceptions;

namespace CRIApp.Application.Services;

public class ChildService : IChildService
{
    private readonly IChildRepository _childRepository;
    private readonly IUserService _userService;

    public ChildService(IChildRepository childRepository, IUserService userService)
    {
        _childRepository = childRepository;
        _userService = userService;
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
            throw new NotFoundException("Child not found");
        
        if (child.PrcId != prcId)
            throw new ValidationException("Child does not belong to this user");

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