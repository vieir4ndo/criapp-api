using CRIApp.Api.Controllers.Requests;
using CRIApp.Api.Controllers.Responses;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dto;
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
    
    [HttpPatch("{childId}")]
    public async Task<ApiResponse<object>> Update(UpdateChildRequest request, [FromRoute] Guid childId)
    {
        request.ValidateAndThrow();
        
        await _service.Update(GetUserId(), childId, request.ToDto());

        return new ApiResponse<object>(true);
    }
    
    [HttpGet("{childId}")]
    public async Task<ApiResponse<ChildDto>> Get([FromRoute] Guid childId)
    {
        var child = await _service.GetAndValidateById(childId, GetUserId());

        return new ApiResponse<ChildDto>(true, new ChildDto(child));
    }
}