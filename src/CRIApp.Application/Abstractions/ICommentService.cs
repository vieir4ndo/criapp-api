using CRIApp.Application.Dtos;

namespace CRIApp.Application.Abstractions;

public interface ICommentService
{
    Task Create(CreateCommentDto dto);
    Task Delete(Guid commentId);
}