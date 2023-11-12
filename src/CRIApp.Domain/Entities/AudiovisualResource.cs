namespace CRIApp.Domain.Entities;

public class AudiovisualResource : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Link { get; private set; }
    
    public Guid AdminId { get; private set; }
    public User Admin { get; private set; }
}