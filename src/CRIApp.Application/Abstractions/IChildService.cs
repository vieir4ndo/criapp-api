using CRIApp.Application.Dtos;

namespace CRIApp.Application.Abstractions;

public interface IChildService
{
    Task Create(CreateChildDto dto);
}