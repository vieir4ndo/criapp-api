using CRIApp.Application.Dtos;

namespace CRIApp.Application.Abstractions;

public interface IChildBookService
{
    Task<ChildBookDto> GetSummaryByChildId(Guid childId, Guid prcId);
    Task<List<AwardDto>> GetAwardsByChildId(Guid childId, Guid prcId);
    Task<List<RecommendationDto>> GetRecommendationsByChildId(Guid childId, Guid prcId);
    Task SaveCheckpoint(Guid childId, Guid prcId, int recommendationId, bool completed);
}
