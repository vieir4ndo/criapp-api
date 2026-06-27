using CRIApp.Domain.Enums;
using CRIApp.Domain.Extensions;

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

        Validate();
    }

    private void Validate()
    {
        ChildBookId.IsEmptyGuid("A notification has to have a child book associated");
        Type.IsInEnum("A notification has to have a valid type");
    }

    public void MarkAsDelivered() => Delivered = true;
    public void MarkAsRead() => Read = true;
}
