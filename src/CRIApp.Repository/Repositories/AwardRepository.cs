using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRIApp.Repository.Repositories;

public class AwardRepository : IAwardRepository
{
    private readonly AppContext Context;

    public AwardRepository(AppContext context)
    {
        Context = context;
    }

    public async Task<IList<Award>> GetAll()
    {
        return await Context.Award.ToListAsync();
    }
}
