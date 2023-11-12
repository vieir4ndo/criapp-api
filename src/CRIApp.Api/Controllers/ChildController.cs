using CRIApp.Api.Controllers.Requests;
using CRIApp.Api.Controllers.Responses;
using CRIApp.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRIApp.Api.Controllers;


[ApiController]
[Authorize]
[Route("child")]
public class ChildController : ControllerBase
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
        
        await _service.Create(request.ToDto());

        return new ApiResponse<object>(true);
    }
}