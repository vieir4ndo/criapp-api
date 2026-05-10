using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;

namespace CRIApp.Application.Dtos;

public class NotificationDto
{
    public Guid Id { get; set; }
    public Guid ChildBookId { get; set; }
    public NotificationType Type { get; set; }
    public DateTime Date { get; set; }
    public bool Read { get; set; }

    public NotificationDto(Notification notification)
    {
        Id = notification.Id;
        ChildBookId = notification.ChildBookId;
        Type = notification.Type;
        Date = notification.Date;
        Read = notification.Read;
    }
}
