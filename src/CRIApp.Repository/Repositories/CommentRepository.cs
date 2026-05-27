using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRIApp.Repository.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly AppContext _context;

    public CommentRepository(AppContext context)
    {
        _context = context;
    }
    
    public async Task Create(Comment comment)
    {
        await _context.Comment.AddAsync(comment);
        await _context.SaveChangesAsync();
    }
    
    public async Task<Comment?> GetById(Guid commentId)
    {
        return await _context.Comment.FindAsync(commentId);
    }

    public async Task Delete(Comment comment)
    {
        _context.Comment.Remove(comment);
        await _context.SaveChangesAsync();
    }

    public async Task<DateTime?> GetLastCreatedAtByPrcAndForum(Guid prcId, Guid forumId)
    {
        return await _context.Comment
            .Where(x => x.PrcId == prcId && x.ForumId == forumId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => (DateTime?)x.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<int> CountByPrcAndForumSince(Guid prcId, Guid forumId, DateTime sinceUtc)
    {
        return await _context.Comment
            .CountAsync(x => x.PrcId == prcId && x.ForumId == forumId && x.CreatedAt >= sinceUtc);
    }
}