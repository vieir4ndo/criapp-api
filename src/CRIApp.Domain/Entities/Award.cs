using CRIApp.Domain.Enums;

namespace CRIApp.Domain.Entities;

public class Award : Entity
{
    public AwardType Type { get; private set; }

    public Guid ChildBookId { get; private set; }
    public ChildBook ChildBook { get; private set; } = null!;

    protected Award() {}

    public Award(AwardType type, Guid childBookId)
    {
        Type = type;
        ChildBookId = childBookId;
    }
}