using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Data;
using CRIApp.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Services;

public class ForumService : IForumService
{
    private readonly IForumRepository _forumRepository;
    private readonly IUserService _userService;
    private readonly ILogger<ForumService> _logger;

    public ForumService(IForumRepository forumRepository, IUserService userService, ILogger<ForumService> logger)
    {
        _forumRepository = forumRepository;
        _userService = userService;
        _logger = logger;
    }

    public async Task Create(CreateForumDto dto)
    {
        _ = await _userService.GetAndValidateById(dto.PrcId);

        await _forumRepository.Create(dto.ToDomainObject());
    }

    public async Task<ForumDto> GetById(Guid forumId)
    {
        var forum = await _forumRepository.GetById(forumId);

        if (forum is null)
        {
            _logger.LogWarning("Forum {ForumId} not found", forumId);
            throw new NotFoundException("Foro no encontrado");
        }
        
        return new ForumDto(forum);
    }

    public async Task<List<ForumDto>> GetAll()
    {
        var forums = await _forumRepository.GetAll();

        var forumsDto = new List<ForumDto>();

        forums.ForEach(x => forumsDto.Add(new ForumDto(x)));

        return forumsDto;
    }

    public async Task Update(Guid forumId, UpdateForumDto dto)
    {
        _ = await _userService.GetAndValidateById(dto.PrcId);
        
        var forum = await _forumRepository.GetById(forumId);

        if (forum is null)
        {
            _logger.LogWarning("Forum {ForumId} not found on update", forumId);
            throw new NotFoundException("Foro no encontrado");
        }
        
        if (forum.PrcId != dto.PrcId)
        {
            _logger.LogWarning("Forum {ForumId} does not belong to user {UserId}", forumId, dto.PrcId);
            throw new ValidationException("Foro no pertenece al usuario");
        }

        forum.Update(dto.Name, dto.Description);
        
        await _forumRepository.Update(forum);
    }
}