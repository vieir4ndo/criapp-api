using CRIApp.Application.Dto;

namespace CRIApp.Application.Abstractions;

public interface IAnswerService
{
    Task Create(CreateAnswerDto dto);
}