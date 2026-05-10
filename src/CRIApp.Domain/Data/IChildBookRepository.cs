using CRIApp.Domain.Entities;

namespace CRIApp.Domain.Data;

public interface IChildBookRepository : IRepository<ChildBook>
{
    Task<ChildBook?> GetByChildId(Guid childId);
    Task Create(ChildBook childBook);
    Task Update(ChildBook childBook);
    Task<IList<ChildBook>> GetAll();
    Task<IList<ChildBook>> GetAllForNotifications();
}
