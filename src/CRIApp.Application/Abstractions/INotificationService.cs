using CRIApp.Application.Dtos;
using CRIApp.Domain.Enums;

namespace CRIApp.Application.Abstractions;

public interface INotificationService
{
    Task<IList<NotificationDto>> GetPendingByUserId(Guid userId);
    Task<NotificationHistoryDto> GetHistoryByUserId(Guid userId, int page, int pageSize);
    Task MarkAsDelivered(Guid notificationId);
    Task MarkAsRead(Guid notificationId);
    Task CreateIfNotExists(Guid childBookId, NotificationType type);
}
