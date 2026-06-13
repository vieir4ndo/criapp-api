using CRIApp.Application.Dtos;

namespace CRIApp.Application.Abstractions;

public interface IChildBookService
{
    Task<ChildBookDto> GetSummaryByChildBookId(Guid childBookId, Guid prcId);
    Task<List<AwardDto>> GetAwardsByChildBookId(Guid childBookId, Guid prcId);
    Task<List<RecommendationDto>> GetRecommendationsByChildBookId(Guid childBookId, Guid prcId);
    Task<SaveCheckpointResultDto> SaveCheckpoint(Guid childBookId, Guid prcId, Guid childBookRecommendationId, bool completed);
}
