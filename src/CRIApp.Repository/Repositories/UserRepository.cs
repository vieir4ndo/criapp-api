using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRIApp.Repository.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppContext Context;

    public UserRepository(AppContext context)
    {
        Context = context;
    }
    public async Task Create(User user)
    {
        await Context.User.AddAsync(user);
        await Context.SaveChangesAsync();
    }

    public async Task<User?> GetByUsername(string email)
    {
        return await Context.User
            .Include(x => x.UserResetPasswordCodes)
            .FirstOrDefaultAsync(x => x.Username.ToLower() == email.ToLower());
    }

    public async Task<User?> GetById(Guid id)
    {
        return await Context.User
            .Include(x => x.Children)
            .Include(x => x.Answers)
            .ThenInclude(x=>x.Question)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User?> GetByIdWithAwardsAndRecommendations(Guid id)
    {
        return await Context.User
            .Include(x => x.Awards)
            .Include(x => x.Recommendations)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task Update(User user)
    {
        Context.User.Update(user);
        await Context.SaveChangesAsync();
    }

    public async Task<IList<User>> GetAll()
    {
        return await Context.User
            .Include(x => x.Awards)
            .Include(x => x.Recommendations)
            .Include(x => x.Answers)
                .ThenInclude(x=>x.Question)
                    .ThenInclude(x=>x.Recommendations)
            .AsSplitQuery()
            .ToListAsync();
    }
}