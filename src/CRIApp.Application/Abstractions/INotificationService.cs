using CRIApp.Application.Dtos;

namespace CRIApp.Application.Abstractions;

public interface INotificationService
{
    Task<IList<NotificationDto>> GetPendingByUserId(Guid userId);
    Task MarkAsDelivered(Guid notificationId);
    Task MarkAsRead(Guid notificationId);
}
