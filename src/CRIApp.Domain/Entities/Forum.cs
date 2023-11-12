namespace CRIApp.Domain.Entities;

public class Forum : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid PrcId { get; private set; }
    public User Prc { get; private set; }
    public List<Comment> Comments { get; private set; }
}