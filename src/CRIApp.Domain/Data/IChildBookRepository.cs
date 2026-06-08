using CRIApp.Domain.Entities;

namespace CRIApp.Domain.Data;

public interface IChildBookRepository : IRepository<ChildBook>
{
    Task<ChildBook?> GetByChildId(Guid childId);
    Task<ChildBook?> GetByChildIdWithRecommendations(Guid childId);
    Task Create(ChildBook childBook);
    Task Update(ChildBook childBook);
    Task<IList<ChildBook>> GetAll();
    Task<IList<ChildBook>> GetAllForNotifications();
    Task<IList<(Recommendation Rec, DateTime AssignedAt)>> GetRecommendationsByChildBookId(Guid childBookId);
    Task<IList<(Award Aw, DateTime AssignedAt)>> GetAwardsByChildBookId(Guid childBookId);
}
