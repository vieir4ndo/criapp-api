namespace CRIApp.Domain.Entities;
using Extensions;

public class AudiovisualResource : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Link { get; private set; }
    
    public Guid AdminId { get; private set; }
    public User Admin { get; private set; }
    
    public AudiovisualResource(string name, string description, string link, Guid adminId)
    {
        Name = name;
        Description = description;
        Link = link;
        AdminId = adminId;
        
        Validate();
    }

    private void Validate()
    {
        Name.IsNullOrEmpty("An audio resource cannot have a null or empty name");
        Description.IsNullOrEmpty("An audio resource cannot have a null or empty description");
        Link.IsNullOrEmpty("An audio resource cannot have a null or empty link");
        AdminId.IsEmptyGuid("An audio resource has to have an administrator associated");
    }
}