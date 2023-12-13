using CRIApp.Domain.Entities;

namespace CRIApp.Domain.Data;

public interface IUserRepository : IRepository<User>
{
    Task Create(User user);
    Task<User?> GetByEmail(string email);
    Task<User?> GetById(Guid id);
    Task Update(User user);
}