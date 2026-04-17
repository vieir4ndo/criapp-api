using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Data;
using CRIApp.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Services;

public class CommentService : ICommentService
{
    private readonly IForumService _forumService;
    private readonly IUserService _userService;
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<CommentService> _logger;

    public CommentService(IForumService forumService, IUserService userService, ICommentRepository commentRepository, ILogger<CommentService> logger)
    {
        _forumService = forumService;
        _userService = userService;
        _commentRepository = commentRepository;
        _logger = logger;
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
            _logger.LogWarning("Comment {CommentId} not found", commentId);
            throw new NotFoundException("Comentario no encontrado");
        }
        
        await _commentRepository.Delete(comment);
    }
}