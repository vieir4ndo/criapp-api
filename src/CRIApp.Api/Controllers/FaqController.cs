using CRIApp.Api.Controllers.Responses;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRIApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("faq")]
public class FaqController : CriAppControllerBase
{
    private readonly IFaqService _service;

    public FaqController(IFaqService service)
    {
        _service = service;
    }
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<ApiResponse<IList<FaqDto>>> GetAll()
    {
        var data = await _service.GetAll();

        return new ApiResponse<IList<FaqDto>>(true, data);
    }
}