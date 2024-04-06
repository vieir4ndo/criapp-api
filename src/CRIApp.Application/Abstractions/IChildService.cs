using CRIApp.Application.Dtos;
using CRIApp.Domain.Entities;

namespace CRIApp.Application.Abstractions;

public interface IChildService
{
    Task Create(CreateChildDto dto);
    Task<List<ChildDto>> GetAllById(Guid prcId);
    Task Update(Guid userId, Guid childId, UpdateChildDto dto);
    Task<Child> GetAndValidateById(Guid id, Guid prcId);
}