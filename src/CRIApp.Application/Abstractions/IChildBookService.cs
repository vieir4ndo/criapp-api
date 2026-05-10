using CRIApp.Application.Dtos;

namespace CRIApp.Application.Abstractions;

public interface IChildBookService
{
    Task<ChildBookDto> GetByChildId(Guid childId, Guid prcId);
    Task SaveCheckpoint(Guid childId, Guid prcId, int recommendationId, bool completed);
}
