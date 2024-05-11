using CRIApp.Application.Dtos;

namespace CRIApp.Application.Abstractions;

public interface IForumService
{
    Task Create(CreateOrUpdateForumDto dto);
    Task<ForumDto> GetById(Guid forumId);
    Task<List<ForumDto>> GetAll();
    Task Update(Guid forumId, CreateOrUpdateForumDto dto);
}