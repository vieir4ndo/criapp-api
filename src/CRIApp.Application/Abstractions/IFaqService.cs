using CRIApp.Application.Dtos;

namespace CRIApp.Application.Abstractions;

public interface IFaqService
{
    Task<IList<FaqDto>> GetAll();
}