using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Data;
using CRIApp.Domain.Exceptions;

namespace CRIApp.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IList<NotificationDto>> GetPendingByUserId(Guid userId)
    {
        var notifications = await _notificationRepository.GetPendingByUserId(userId);
        return notifications.Select(n => new NotificationDto(n)).ToList();
    }

    public async Task MarkAsDelivered(Guid notificationId)
    {
        var notification = await _notificationRepository.GetById(notificationId)
            ?? throw new NotFoundException("Notification not found");

        notification.MarkAsDelivered();
        await _notificationRepository.Update(notification);
    }

    public async Task MarkAsRead(Guid notificationId)
    {
        var notification = await _notificationRepository.GetById(notificationId)
            ?? throw new NotFoundException("Notification not found");

        notification.MarkAsRead();
        await _notificationRepository.Update(notification);
    }
}
