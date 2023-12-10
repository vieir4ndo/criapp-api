using CRIApp.Api.Controllers.Responses;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
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
    public async Task<ApiResponse<QuestionDto>> GetAllQuestionsToReply()
    {
        var data = await _service.GetAllQuestionsToReply(GetUserId());

        return new ApiResponse<QuestionDto>(true, data);
    }
}