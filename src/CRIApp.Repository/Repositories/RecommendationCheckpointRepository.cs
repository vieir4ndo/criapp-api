using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRIApp.Repository.Repositories;

public class RecommendationCheckpointRepository : IRecommendationCheckpointRepository
{
    private readonly AppContext _context;

    public RecommendationCheckpointRepository(AppContext context)
    {
        _context = context;
    }

    public async Task<RecommendationCheckpoint?> GetByDateAndIds(int recommendationId, Guid childBookId, DateTime date)
    {
        return await _context.RecommendationCheckpoint
            .FirstOrDefaultAsync(x =>
                x.RecommendationId == recommendationId &&
                x.ChildBookId == childBookId &&
                x.Date == date);
    }

    public async Task<IList<RecommendationCheckpoint>> GetByChildBookIdAndDate(Guid childBookId, DateTime date)
    {
        return await _context.RecommendationCheckpoint
            .Where(x => x.ChildBookId == childBookId && x.Date == date)
            .ToListAsync();
    }

    public async Task<int> CountDistinctCompletedDaysInPeriod(int recommendationId, Guid childBookId, DateTime from, DateTime to)
    {
        return await _context.RecommendationCheckpoint
            .Where(x =>
                x.RecommendationId == recommendationId &&
                x.ChildBookId == childBookId &&
                x.Completed &&
                x.Date >= from &&
                x.Date <= to)
            .Select(x => x.Date)
            .Distinct()
            .CountAsync();
    }

    public async Task Create(RecommendationCheckpoint checkpoint)
    {
        await _context.RecommendationCheckpoint.AddAsync(checkpoint);
        await _context.SaveChangesAsync();
    }

    public async Task<Dictionary<int, int>> GetCompletedCountByChildBook(Guid childBookId)
    {
        return await _context.RecommendationCheckpoint
            .Where(x => x.ChildBookId == childBookId && x.Completed)
            .GroupBy(x => x.RecommendationId)
            .Select(g => new { RecommendationId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.RecommendationId, x => x.Count);
    }
}
