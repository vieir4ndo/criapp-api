using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Application.Resources;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Microsoft.Extensions.Localization;

namespace CRIApp.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IStringLocalizer<MessageTemplates> _localizer;

    public NotificationService(
        INotificationRepository notificationRepository,
        IStringLocalizer<MessageTemplates> localizer)
    {
        _notificationRepository = notificationRepository;
        _localizer = localizer;
    }

    public async Task<IList<NotificationDto>> GetPendingByUserId(Guid userId)
    {
        var notifications = await _notificationRepository.GetPendingByUserId(userId);
        return notifications.Select(n =>
        {
            var childName = n.ChildBook?.Child?.Name ?? string.Empty;
            var dto = new NotificationDto(n);
            (dto.Title, dto.Body) = ResolveContent(n.Type, childName);
            return dto;
        }).ToList();
    }

    private (string Title, string Body) ResolveContent(NotificationType type, string childName) => type switch
    {
        NotificationType.Motivational => (
            _localizer["Notification_Motivational_Heading"].Value,
            string.Format(_localizer["Notification_Motivational_Body"].Value, childName)
        ),
        NotificationType.PointsRisk => (
            _localizer["Notification_PointsRisk_Heading"].Value,
            string.Format(_localizer["Notification_PointsRisk_Body"].Value, childName)
        ),
        NotificationType.ReEvaluation => (
            _localizer["Notification_ReEvaluation_Heading"].Value,
            string.Format(_localizer["Notification_ReEvaluation_Body"].Value, childName)
        ),
        NotificationType.NewAward => (
            _localizer["Notification_NewAward_Heading"].Value,
            string.Format(_localizer["Notification_NewAward_Body"].Value, childName)
        ),
        NotificationType.NewRecommendations => (
            _localizer["Notification_NewRecommendations_Heading"].Value,
            string.Format(_localizer["Notification_NewRecommendations_Body"].Value, childName)
        ),
        _ => (string.Empty, string.Empty)
    };

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

    public async Task CreateIfNotExists(Guid childBookId, NotificationType type)
    {
        var today = DateTime.UtcNow.Date;
        var alreadyExists = await _notificationRepository.ExistsForToday(childBookId, type, today);
        if (!alreadyExists)
            await _notificationRepository.Create(new Notification(childBookId, type));
    }
}
