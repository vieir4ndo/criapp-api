using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Data;

namespace CRIApp.Application.Services;

public class AudiovisualResourceService : IAudiovisualResourceService
{
    private readonly IAudiovisualResourceRepository _repository;

    public AudiovisualResourceService(IAudiovisualResourceRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<IList<AudiovisualResourceDto>> GetAll()
    {
        var audiovisualResources = await _repository.GetAll();

        return audiovisualResources.Select(audiovisualResource => new AudiovisualResourceDto(audiovisualResource)).ToList();
    }
}