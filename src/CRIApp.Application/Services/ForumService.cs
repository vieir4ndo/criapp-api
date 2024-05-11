using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Data;
using CRIApp.Domain.Exceptions;

namespace CRIApp.Application.Services;

public class ForumService : IForumService
{
    private readonly IForumRepository _forumRepository;
    private readonly IUserService _userService;

    public ForumService(IForumRepository forumRepository, IUserService userService)
    {
        _forumRepository = forumRepository;
        _userService = userService;
    }

    public async Task Create(CreateOrUpdateForumDto dto)
    {
        _ = await _userService.GetAndValidateById(dto.PrcId);

        await _forumRepository.Create(dto.ToDomainObject());
    }

    public async Task<ForumDto> GetById(Guid forumId)
    {
        var forum = await _forumRepository.GetById(forumId);

        if (forum is null)
            throw new NotFoundException("Foro no encontrado");
        
        return new ForumDto(forum);
    }

    public async Task<List<ForumDto>> GetAll()
    {
        var forums = await _forumRepository.GetAll();

        var forumsDto = new List<ForumDto>();

        forums.ForEach(x => forumsDto.Add(new ForumDto(x)));

        return forumsDto;
    }

    public async Task Update(Guid forumId, CreateOrUpdateForumDto dto)
    {
        _ = await _userService.GetAndValidateById(dto.PrcId);
        
        var forum = await _forumRepository.GetById(forumId);

        if (forum is null)
            throw new NotFoundException("Foro no encontrado");
        
        if (forum.PrcId != dto.PrcId)
            throw new ValidationException("Foro no pertenece al usuario");

        forum.Update(dto.Name, dto.Description);
        
        await _forumRepository.Update(forum);
    }
}