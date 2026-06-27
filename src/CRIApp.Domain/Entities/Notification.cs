using CRIApp.Domain.Enums;

namespace CRIApp.Domain.Entities;

public class Notification : Entity
{
    public Guid ChildBookId { get; private set; }
    public NotificationType Type { get; private set; }
    public DateTime Date { get; private set; }
    public bool Delivered { get; private set; }
    public bool Read { get; private set; }
    public ChildBook? ChildBook { get; private set; }

    public Notification(Guid childBookId, NotificationType type)
    {
        ChildBookId = childBookId;
        Type = type;
        Date = DateTime.UtcNow.Date;
        Delivered = false;
        Read = false;
    }

    public void MarkAsDelivered() => Delivered = true;
    public void MarkAsRead() => Read = true;
}
