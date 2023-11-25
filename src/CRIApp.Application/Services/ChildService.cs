using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Data;

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
}