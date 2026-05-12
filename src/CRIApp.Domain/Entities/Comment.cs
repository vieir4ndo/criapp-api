using CRIApp.Domain.Extensions; 
namespace CRIApp.Domain.Entities;
public class Comment : Entity
{
    public string Text { get; private set; }
    
    public Guid ForumId { get; private set; }
    public Forum Forum { get; private set; }
    
    public Guid PrcId { get; private set; }
    public User Prc { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Comment(string text, Guid forumId, Guid prcId)
    {
        Text = text;
        ForumId = forumId;
        PrcId = prcId;
        CreatedAt = DateTime.UtcNow;
        
        Text.IsNullOrEmpty("A comment cannot have a null or empty text");
        ForumId.IsEmptyGuid("A comment has to have a Forum associated");
        PrcId.IsEmptyGuid("A comment has to have a PRC associated");
    }
}