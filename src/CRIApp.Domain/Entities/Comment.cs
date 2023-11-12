namespace CRIApp.Domain.Entities;

public class Comment : Entity
{
    public string Text { get; private set; }
    
    public Guid ForumId { get; private set; }
    public Forum Forum { get; private set; }
    
    public Guid PrcId { get; private set; }
    public User Prc { get; private set; }
}