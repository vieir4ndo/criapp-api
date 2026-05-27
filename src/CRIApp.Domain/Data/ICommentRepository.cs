using CRIApp.Domain.Entities;

namespace CRIApp.Domain.Data;

public interface ICommentRepository
{
    Task Create(Comment comment);
    Task Delete(Comment comment);
    Task<Comment?> GetById(Guid commentId);
    Task<DateTime?> GetLastCreatedAtByPrcAndForum(Guid prcId, Guid forumId);
    Task<int> CountByPrcAndForumSince(Guid prcId, Guid forumId, DateTime sinceUtc);
}