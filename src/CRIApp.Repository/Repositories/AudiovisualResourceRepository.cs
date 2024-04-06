using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRIApp.Repository.Repositories;

public class AudiovisualResourceRepository : IAudiovisualResourceRepository
{
    private readonly AppContext Context;

    public AudiovisualResourceRepository(AppContext context)
    {
        Context = context;
    }
    
    public async Task<IList<AudiovisualResource>> GetAll()
    {
        return await Context.AudiovisualResource.ToListAsync();
    }
}