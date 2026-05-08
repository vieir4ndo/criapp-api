using CRIApp.Api.Controllers.Requests;
using CRIApp.Api.Controllers.Responses;
using CRIApp.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRIApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("answer")]
    public class AnswerController : CriAppControllerBase
{
    private readonly IAnswerService _service;

    public AnswerController(IAnswerService answerService, ILogger<AnswerController> logger) : base(logger)
    {
        _service = answerService;
    }

    [HttpPost]
    public async Task<ApiResponse<object>> Create(CreateAnswerRequest request)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        
        ValidateAndLog(request);
        
        await _service.Create(request.ToDto(userIdClaim!));

        return new ApiResponse<object>(true);
    }
}