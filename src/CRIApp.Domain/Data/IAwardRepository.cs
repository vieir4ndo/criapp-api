using CRIApp.Domain.Entities;

namespace CRIApp.Domain.Data;

public interface IAwardRepository : IRepository<Award>
{
    Task CreateRange(IEnumerable<Award> awards);
}
