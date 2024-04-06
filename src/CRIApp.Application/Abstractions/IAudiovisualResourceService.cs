using CRIApp.Application.Dtos;

namespace CRIApp.Application.Abstractions;

public interface IAudiovisualResourceService
{
    Task<IList<AudiovisualResourceDto>> GetAll();
}