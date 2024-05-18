using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Data;
using CRIApp.Domain.Exceptions;

namespace CRIApp.Application.Services;

public class CommentService : ICommentService
{
    private readonly IForumService _forumService;
    private readonly IUserService _userService;
    private readonly ICommentRepository _commentRepository;

    public CommentService(IForumService forumService, IUserService userService, ICommentRepository commentRepository)
    {
        _forumService = forumService;
        _userService = userService;
        _commentRepository = commentRepository;
    }
    
    public async Task Create(CreateCommentDto dto)
    {
        _ = await _userService.GetAndValidateById(dto.PrcId);
        _ = await _forumService.GetById(dto.ForumId);

        await _commentRepository.Create(dto.ToDomainObject());
    }

    public async Task Delete(Guid commentId)
    {
        var comment = await _commentRepository.GetById(commentId);

        if (comment is null)
        {
            throw new NotFoundException("Comentario no encontrado");
        }
        
        await _commentRepository.Delete(comment);
    }
}