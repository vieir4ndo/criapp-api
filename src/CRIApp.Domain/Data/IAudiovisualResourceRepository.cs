using CRIApp.Domain.Entities;

namespace CRIApp.Domain.Data;

public interface IAudiovisualResourceRepository : IRepository<AudiovisualResource>
{
    Task<IList<AudiovisualResource>> GetAll();
}