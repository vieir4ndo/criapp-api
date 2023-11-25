using CRIApp.Api.Controllers.Requests;
using CRIApp.Api.Controllers.Responses;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRIApp.Api.Controllers;


[ApiController]
[Authorize]
[Route("child")]
public class ChildController : CriAppControllerBase
{
    private readonly IChildService _service;

    public ChildController(IChildService service)
    {
        _service = service;
    }
    
    [HttpPost]
    public async Task<ApiResponse<object>> Create(CreateChildRequest request)
    {
        request.ValidateAndThrow();
        
        await _service.Create(request.ToDto(GetUserId()));

        return new ApiResponse<object>(true);
    }
    
    [HttpGet]
    public async Task<ApiResponse<List<ChildDto>>> GetAllById()
    {
        var data = await _service.GetAllById(GetUserId());

        return new ApiResponse<List<ChildDto>>(true, data);
    }
}