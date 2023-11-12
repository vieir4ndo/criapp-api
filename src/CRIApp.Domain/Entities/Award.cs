using CRIApp.Domain.Enums;

namespace CRIApp.Domain.Entities;

public class Award : Entity
{
    public AwardType Type { get; private set; }
    
    public Guid PrcId { get; private set; }
    public User Prc { get; private set; }
}