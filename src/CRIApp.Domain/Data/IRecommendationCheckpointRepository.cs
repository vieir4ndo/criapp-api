using CRIApp.Domain.Entities;

namespace CRIApp.Domain.Data;

public interface IRecommendationCheckpointRepository
{
    Task<RecommendationCheckpoint?> GetByDateAndIds(int recommendationId, Guid childBookId, DateTime date);
    Task<IList<RecommendationCheckpoint>> GetByChildBookIdAndDate(Guid childBookId, DateTime date);
    Task Create(RecommendationCheckpoint checkpoint);
}
