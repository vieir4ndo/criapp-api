using CRIApp.Application.Dtos;

namespace CRIApp.Application.Abstractions;

public interface IChildBookService
{
    Task<ChildBookDto> GetByChildId(Guid childId, Guid prcId);
}
