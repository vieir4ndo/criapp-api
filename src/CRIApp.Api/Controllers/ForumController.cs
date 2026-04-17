using CRIApp.Api.Controllers.Requests;
using CRIApp.Api.Controllers.Responses;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CRIApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("forum")]
public class ForumController : CriAppControllerBase
{
    private readonly IForumService _service;

    public ForumController(IForumService service, ILogger<ForumController> logger) : base(logger)
    {
        _service = service;
    }
    
    [HttpPost]
    public async Task<ApiResponse<object>> Create(CreateForumRequest request)
    {
        ValidateAndLog(request);
        
        await _service.Create(request.ToDto(GetUserId()));

        return new ApiResponse<object>(true);
    }
    
    [HttpGet("{forumId}")]
    public async Task<ApiResponse<ForumDto>> GetById([FromRoute] Guid forumId)
    {
        var forum = await _service.GetById(forumId);

        return new ApiResponse<ForumDto>(true, forum);
    }
    
    [HttpGet]
    public async Task<ApiResponse<ForumDto>> GetAll()
    {
        var forums = await _service.GetAll();

        return new ApiResponse<ForumDto>(true, forums);
    }
    
    [HttpPatch("{forumId}")]
    public async Task<ApiResponse<object>> Update([FromRoute] Guid forumId, UpdateForumRequest request)
    {
        ValidateAndLog(request);
        
        await _service.Update(forumId, request.ToDto(GetUserId()));

        return new ApiResponse<object>(true);
    }
    
    [HttpGet("{forumId}/comments")]
    public async Task<ApiResponse<CommentDto>> GetCommentsById([FromRoute] Guid forumId)
    {
        var forum = await _service.GetById(forumId);
    
        return new ApiResponse<CommentDto>(true, forum.Comments);
    }
}