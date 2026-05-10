using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;

namespace CRIApp.Domain.Data;

public interface INotificationRepository
{
    Task<bool> ExistsForToday(Guid childBookId, NotificationType type, DateTime date);
    Task Create(Notification notification);
    Task<IList<Notification>> GetPendingByChildBookIds(IList<Guid> childBookIds);
    Task<IList<Notification>> GetPendingByUserId(Guid userId);
    Task<IList<Notification>> GetHistoryByUserId(Guid userId);
    Task<Notification?> GetById(Guid id);
    Task Update(Notification notification);
}
