using CRIApp.Domain.Entities;

namespace CRIApp.Domain.Data;

public interface ICommentRepository
{
    Task Create(Comment comment);
    Task Delete(Comment comment);
    Task<Comment?> GetById(Guid commentId);
}