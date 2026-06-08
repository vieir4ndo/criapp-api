using CRIApp.Domain.Entities;

namespace CRIApp.Domain.Data;

public interface IRecommendationCheckpointRepository
{
    Task<RecommendationCheckpoint?> GetByDateAndIds(int recommendationId, Guid childBookId, DateTime date);
    Task<IList<RecommendationCheckpoint>> GetByChildBookIdAndDate(Guid childBookId, DateTime date);
    Task<int> CountDistinctCompletedDaysInPeriod(int recommendationId, Guid childBookId, DateTime from, DateTime to);
    Task<Dictionary<int, int>> GetDistinctCompletedDaysInWindowByChildBook(Guid childBookId, DateTime from, DateTime to);
    Task Create(RecommendationCheckpoint checkpoint);
}
