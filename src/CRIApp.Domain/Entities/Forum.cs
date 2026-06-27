using CRIApp.Domain.Extensions;

namespace CRIApp.Domain.Entities;

public class Forum : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid PrcId { get; private set; }
    public User Prc { get; private set; } = null!;
    public List<Comment> Comments { get; private set; }

    public Forum(string name, string description, Guid prcId)
    {
        Name = name.ToTitleCase();
        Description = description;
        PrcId = prcId;
        Comments = [];
        
        Name.IsNullOrEmpty("A forum cannot have a null or empty name");
        Description.IsNullOrEmpty("A forum cannot have a null or empty description");
        PrcId.IsEmptyGuid("A forum has to have a PRC associated");
    }
    
    public void Update(string? name, string? description)
    {
        Name = name?.ToTitleCase() ?? Name;
        Description = description ?? Description;
    }

    public void OrderCommentsByCreatedAt()
    {
        Comments = Comments.OrderBy(c => c.CreatedAt).ToList();
    }
}