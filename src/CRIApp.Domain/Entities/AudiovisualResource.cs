namespace CRIApp.Domain.Entities;
using Extensions;

public class AudiovisualResource : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Iframe { get; private set; }
    public string? NameEn { get; private set; }
    public string? DescriptionEn { get; private set; }
    public string? NamePt { get; private set; }
    public string? DescriptionPt { get; private set; }

    protected AudiovisualResource() {}

    public AudiovisualResource(string name, string description, string iframe, Guid adminId)
    {
        Name = name;
        Description = description;
        Iframe = iframe;
        
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
        Iframe.IsNullOrEmpty("An audio resource cannot have a null or empty iframe");
    }
}