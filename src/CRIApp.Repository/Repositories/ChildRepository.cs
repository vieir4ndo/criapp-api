using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRIApp.Repository.Repositories;

public class ChildRepository : IChildRepository
{
    private readonly AppContext Context;

    public ChildRepository(AppContext context)
    {
        Context = context;
    }
    
    public async Task<Child?> GetById(Guid id)
    {
        return await Context.Child.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task Create(Child child)
    {
        await Context.Child.AddAsync(child);
        await Context.SaveChangesAsync();
    }
}