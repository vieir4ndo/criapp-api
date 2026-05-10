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

    public async Task Create(RecommendationCheckpoint checkpoint)
    {
        await _context.RecommendationCheckpoint.AddAsync(checkpoint);
        await _context.SaveChangesAsync();
    }
}
