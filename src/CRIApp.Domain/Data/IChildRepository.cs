using CRIApp.Domain.Entities;

namespace CRIApp.Domain.Data;

public interface IChildRepository : IRepository<Child>
{
    Task<Child?> GetById(Guid id);
    Task Create(Child child);
    
    Task<List<Child>> GetAllById(Guid prcId);
}