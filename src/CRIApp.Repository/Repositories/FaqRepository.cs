using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRIApp.Repository.Repositories;

public class FaqRepository : IFaqRepository
{
    private readonly AppContext Context;

    public FaqRepository(AppContext context)
    {
        Context = context;
    }
    
    public async Task<IList<Faq>> GetAll()
    {
        return await Context.Faq.OrderBy(x=>x.Id).ToListAsync();
    }
}