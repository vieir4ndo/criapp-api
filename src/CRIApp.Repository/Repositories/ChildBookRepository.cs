using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRIApp.Repository.Repositories;

public class ChildBookRepository : IChildBookRepository
{
    private readonly AppContext _context;

    public ChildBookRepository(AppContext context)
    {
        _context = context;
    }

    public async Task<ChildBook?> GetByChildId(Guid childId)
    {
        return await _context.ChildBook
            .Include(x => x.Awards)
            .Include(x => x.Recommendations)
            .FirstOrDefaultAsync(x => x.ChildId == childId);
    }

    public async Task Create(ChildBook childBook)
    {
        await _context.ChildBook.AddAsync(childBook);
        await _context.SaveChangesAsync();
    }

    public async Task Update(ChildBook childBook)
    {
        _context.ChildBook.Update(childBook);
        await _context.SaveChangesAsync();
    }

    public async Task<IList<ChildBook>> GetAll()
    {
        return await _context.ChildBook
            .Include(x => x.Awards)
            .Include(x => x.Recommendations)
            .Include(x => x.RecommendationCheckpoints)
            .Include(x => x.Child)
                .ThenInclude(c => c.Answers)
                    .ThenInclude(a => a.Question)
                        .ThenInclude(q => q.Recommendations)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<IList<ChildBook>> GetAllForNotifications()
    {
        return await _context.ChildBook
            .Include(x => x.RecommendationCheckpoints)
            .Include(x => x.Child)
                .ThenInclude(c => c.Answers)
            .Include(x => x.Child)
                .ThenInclude(c => c.Prc)
            .AsSplitQuery()
            .ToListAsync();
    }
}
