using CRIApp.Domain.Entities;

namespace CRIApp.Domain.Data;

public interface IUserRepository : IRepository<User>
{
    Task Create(User user);
    Task<User?> GetByUsername(string email);
    Task<User?> GetById(Guid id);
    Task<User?> GetByIdWithAwardsAndRecommendations(Guid id);
    Task Update(User user);
    Task<IList<User>> GetAll();
}