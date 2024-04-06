using CRIApp.Domain.Entities;

namespace CRIApp.Application.Dtos;

public class AudiovisualResourceDto
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Link { get; private set; }

    public AudiovisualResourceDto(AudiovisualResource audiovisualResource)
    {
        Id = audiovisualResource.Id;
        Name = audiovisualResource.Name;
        Description = audiovisualResource.Description;
        Link = audiovisualResource.Link;
    }
}