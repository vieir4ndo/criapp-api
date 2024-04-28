namespace CRIApp.Domain.Entities;
using Extensions;

public class AudiovisualResource : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Iframe { get; private set; }
    
    public Guid AdminId { get; private set; }
    public User Admin { get; private set; }
    
    public AudiovisualResource(string name, string description, string iframe, Guid adminId)
    {
        Name = name;
        Description = description;
        Iframe = iframe;
        AdminId = adminId;
        
        Validate();
    }

    private void Validate()
    {
        Name.IsNullOrEmpty("An audio resource cannot have a null or empty name");
        Description.IsNullOrEmpty("An audio resource cannot have a null or empty description");
        Iframe.IsNullOrEmpty("An audio resource cannot have a null or empty iframe");
        AdminId.IsEmptyGuid("An audio resource has to have an administrator associated");
    }
}