using CRIApp.Domain.Entities;

namespace CRIApp.Domain.Data;

public interface IFaqRepository
{
    Task<IList<Faq>> GetAll();
}