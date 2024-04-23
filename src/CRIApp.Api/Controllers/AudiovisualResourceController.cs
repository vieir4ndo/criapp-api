using CRIApp.Api.Controllers.Responses;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRIApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("audiovisual-resource")]
public class AudiovisualResourceController : CriAppControllerBase
{
    private readonly IAudiovisualResourceService _service;

    public AudiovisualResourceController(IAudiovisualResourceService service)
    {
        _service = service;
    }
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<ApiResponse<AudiovisualResourceDto>> GetAll()
    {
        var data = await _service.GetAll();

        return new ApiResponse<AudiovisualResourceDto>(true, data);
    }
}