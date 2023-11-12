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
}