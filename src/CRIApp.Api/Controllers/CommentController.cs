using CRIApp.Api.Controllers.Requests;
using CRIApp.Api.Controllers.Responses;
using CRIApp.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CRIApp.Api.Controllers;

public class CommentController : CriAppControllerBase
{
    private readonly ICommentService _service;

    public CommentController(ICommentService service)
    {
        _service = service;
    }
    
    [HttpPost]
    public async Task<ApiResponse<object>> Create(CreateCommentRequest request)
    {
        request.ValidateAndThrow();

        await _service.Create(request.ToDto(GetUserId()));
        
        return new ApiResponse<object>(true);
    }
    
    [HttpDelete("{forumId}")]
    public async Task<ApiResponse<object>> Delete([FromRoute] Guid forumId)
    {
        await _service.Delete(forumId);

        return new ApiResponse<object>(true);
    }
}