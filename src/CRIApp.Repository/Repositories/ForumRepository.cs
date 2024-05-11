using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRIApp.Repository.Repositories;

public class ForumRepository : IForumRepository
{
    private readonly AppContext _context;

    public ForumRepository(AppContext context)
    {
        _context = context;
    }
    public async Task<Forum?> GetById(Guid id)
    {
        return await _context.Forum
            .Where(x => x.Id == id)
            .Include(x => x.Comments)
            .Include(x =>x.Prc)
            .FirstOrDefaultAsync();
    }

    public async Task Create(Forum forum)
    {
        await _context.Forum.AddAsync(forum);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Forum>> GetAll()
    {
        return await _context.Forum
            .Include(x=>x.Prc)
            .ToListAsync();
    }

    public async Task Update(Forum forum)
    {
        _context.Forum.Update(forum);
        await _context.SaveChangesAsync();
    }
}