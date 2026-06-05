using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CRIApp.Repository.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppContext _context;

    public RefreshTokenRepository(AppContext context)
    {
        _context = context;
    }

    public async Task Create(RefreshToken token)
    {
        await _context.RefreshToken.AddAsync(token);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByTokenHash(string tokenHash)
    {
        return await _context.RefreshToken
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);
    }

    public async Task Delete(RefreshToken token)
    {
        _context.RefreshToken.Remove(token);
        await _context.SaveChangesAsync();
    }

    public async Task<int> DeleteByUserId(Guid userId)
    {
        return await _context.RefreshToken
            .Where(x => x.UserId == userId)
            .ExecuteDeleteAsync();
    }
}
