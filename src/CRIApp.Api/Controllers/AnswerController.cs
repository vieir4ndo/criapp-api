using CRIApp.Api.Controllers.Requests;
using CRIApp.Api.Controllers.Responses;
using CRIApp.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRIApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("answer")]
public class AnswerController : ControllerBase
{
    private readonly IAnswerService _service;

    public AnswerController(IAnswerService answerService)
    {
        _service = answerService;
    }

    [HttpPost]
    public async Task<ApiResponse<object>> Create(CreateAnswerRequest request)
    {
        request.ValidateAndThrow();
        
        await _service.Create(request.ToDto());

        return new ApiResponse<object>(true);
    }
}