using CRIApp.Domain.Entities;
using System.Globalization;

namespace CRIApp.Application.Dtos;

public class AudiovisualResourceDto
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Link { get; private set; }

    public AudiovisualResourceDto(AudiovisualResource audiovisualResource)
    {
        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        Id = audiovisualResource.Id;
        Name = lang switch
        {
            "en" => audiovisualResource.NameEn ?? audiovisualResource.Name,
            "pt" => audiovisualResource.NamePt ?? audiovisualResource.Name,
            _ => audiovisualResource.Name
        };
        Description = lang switch
        {
            "en" => audiovisualResource.DescriptionEn ?? audiovisualResource.Description,
            "pt" => audiovisualResource.DescriptionPt ?? audiovisualResource.Description,
            _ => audiovisualResource.Description
        };
        Link = audiovisualResource.Link;
    }
}