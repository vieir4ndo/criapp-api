namespace CRIApp.Domain.Entities;
using Extensions;

public class AudiovisualResource : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Link { get; private set; }
    public string? NameEn { get; private set; }
    public string? DescriptionEn { get; private set; }
    public string? NamePt { get; private set; }
    public string? DescriptionPt { get; private set; }

    protected AudiovisualResource() {}

    public AudiovisualResource(string name, string description, string link, Guid adminId)
    {
        Name = name;
        Description = description;
        Link = link;
        
        Validate();
    }

    public AudiovisualResource WithTranslations(string? nameEn, string? descriptionEn, string? namePt, string? descriptionPt)
    {
        NameEn = nameEn;
        DescriptionEn = descriptionEn;
        NamePt = namePt;
        DescriptionPt = descriptionPt;
        return this;
    }

    private void Validate()
    {
        Name.IsNullOrEmpty("An audio resource cannot have a null or empty name");
        Description.IsNullOrEmpty("An audio resource cannot have a null or empty description");
        Link.IsNullOrEmpty("An audio resource cannot have a null or empty link");
    }
}