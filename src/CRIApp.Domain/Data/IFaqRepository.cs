using CRIApp.Domain.Entities;

namespace CRIApp.Domain.Data;

public interface IFaqRepository : IRepository<Faq>
{
    Task<IList<Faq>> GetAll();
}