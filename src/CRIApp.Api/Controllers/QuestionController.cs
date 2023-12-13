using CRIApp.Api.Controllers.Requests;
using CRIApp.Api.Controllers.Responses;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRIApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("question")]
public class QuestionController : CriAppControllerBase
{
    private readonly IQuestionService _service;

    public QuestionController(IQuestionService service)
    {
        _service = service;
    }
    
    [HttpGet]
    public async Task<ApiResponse<QuestionDto>> GetQuestionsToReply([FromQuery]GetQuestionsToReplyRequest request)
    {
        request.ValidateAndThrow();
        
        var data = await _service.GetQuestionsToReply(GetUserId(), request.ChildId);

        return new ApiResponse<QuestionDto>(true, data);
    }
}