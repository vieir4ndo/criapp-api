using CRIApp.Domain.Entities;

namespace CRIApp.Domain.Data;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task Create(RefreshToken token);
    Task<RefreshToken?> GetByTokenHash(string tokenHash);
    Task Delete(RefreshToken token);
    Task<int> DeleteByUserId(Guid userId);
}
