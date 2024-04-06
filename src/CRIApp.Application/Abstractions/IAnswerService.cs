using CRIApp.Application.Dtos;

namespace CRIApp.Application.Abstractions;

public interface IAnswerService
{
    Task Create(CreateAnswerDto dto);
}