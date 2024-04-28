using CRIApp.Domain.Entities;

namespace CRIApp.Application.Dtos;

public class AudiovisualResourceDto
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Iframe { get; private set; }

    public AudiovisualResourceDto(AudiovisualResource audiovisualResource)
    {
        Id = audiovisualResource.Id;
        Name = audiovisualResource.Name;
        Description = audiovisualResource.Description;
        Iframe = audiovisualResource.Iframe;
    }
}