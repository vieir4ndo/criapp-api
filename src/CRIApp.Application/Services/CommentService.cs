using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Application.Resources;
using CRIApp.Domain.Data;
using CRIApp.Domain.Exceptions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Services;

public class CommentService : ICommentService
{
    private static readonly TimeSpan MinIntervalBetweenComments = TimeSpan.FromSeconds(8);
    private static readonly TimeSpan RateLimitWindow = TimeSpan.FromMinutes(1);
    private const int MaxCommentsPerWindow = 5;

    private readonly IForumService _forumService;
    private readonly IUserService _userService;
    private readonly ICommentRepository _commentRepository;
    private readonly ITextModerationService _textModerationService;
    private readonly ILogger<CommentService> _logger;
    private readonly IStringLocalizer<AppMessages> _localizer;

    public CommentService(
        IForumService forumService,
        IUserService userService,
        ICommentRepository commentRepository,
        ITextModerationService textModerationService,
        ILogger<CommentService> logger,
        IStringLocalizer<AppMessages> localizer)
    {
        _forumService = forumService;
        _userService = userService;
        _commentRepository = commentRepository;
        _textModerationService = textModerationService;
        _logger = logger;
        _localizer = localizer;
    }
    
    public async Task Create(CreateCommentDto dto)
    {
        _ = await _userService.GetAndValidateById(dto.PrcId);
        _ = await _forumService.GetById(dto.ForumId);

        if (_textModerationService.ContainsBlockedContent(dto.Text))
            throw new ValidationException(_localizer["CommentContainsBadWords"]);

        var now = DateTime.UtcNow;
        var lastCommentAt = await _commentRepository.GetLastCreatedAtByPrcAndForum(dto.PrcId, dto.ForumId);
        if (lastCommentAt.HasValue && now - lastCommentAt.Value < MinIntervalBetweenComments)
            throw new ValidationException(_localizer["CommentTooFrequent"]);

        var since = now.Subtract(RateLimitWindow);
        var commentsInWindow = await _commentRepository.CountByPrcAndForumSince(dto.PrcId, dto.ForumId, since);
        if (commentsInWindow >= MaxCommentsPerWindow)
            throw new ValidationException(_localizer["CommentTooManyRequests"]);

        await _commentRepository.Create(dto.ToDomainObject());
    }

    public async Task Delete(Guid commentId)
    {
        var comment = await _commentRepository.GetById(commentId);

        if (comment is null)
        {
            _logger.LogWarning("Comment {CommentId} not found", commentId);
            throw new NotFoundException(_localizer["CommentNotFound"]);
        }
        
        await _commentRepository.Delete(comment);
    }
}