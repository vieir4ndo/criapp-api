using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRIApp.Repository.Repositories;

public class UserResetPasswordCodeRepository : IUserResetPasswordCodeRepository
{
    private readonly AppContext Context;

    public UserResetPasswordCodeRepository(AppContext context)
    {
        Context = context;
    }
    public async Task Create(UserResetPasswordCode code)
    {
        await Context.UserResetPasswordCode.AddAsync(code);
        await Context.SaveChangesAsync();
    }
    
    public async Task<UserResetPasswordCode?> GetLastCodeByUserId(Guid userId)
    {
        return await Context.UserResetPasswordCode
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<int> DeleteCodesOlderThan(DateTime threshold)
    {
        return await Context.UserResetPasswordCode
            .Where(x => x.CreatedAt < threshold)
            .ExecuteDeleteAsync();
    }
}