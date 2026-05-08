using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;

namespace CRIApp.Repository.Repositories;

public class AwardRepository : IAwardRepository
{
    private readonly AppContext Context;

    public AwardRepository(AppContext context)
    {
        Context = context;
    }

    public async Task CreateRange(IEnumerable<Award> awards)
    {
        await Context.Award.AddRangeAsync(awards);
        await Context.SaveChangesAsync();
    }
}
