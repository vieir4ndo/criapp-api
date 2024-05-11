using CRIApp.Domain.Entities;

namespace CRIApp.Domain.Data;

public interface IForumRepository
{
    Task<Forum?> GetById(Guid id);
    Task Create(Forum forum);
    Task<List<Forum>> GetAll();
    Task Update(Forum forum);
}