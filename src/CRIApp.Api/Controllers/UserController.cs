using CRIApp.Api.Controllers.Requests;
using CRIApp.Api.Controllers.Responses;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CRIApp.Api.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }
    
    [HttpPost]
    public async Task<ApiResponse<object>> Create(CreateUserRequest request)
    {
        request.ValidateAndThrow();
        
        await _service.Create(request.ToDto());

        return new ApiResponse<object>(true);
    }

    [HttpPost("login")]
    public async Task<ApiResponse<TokenDto>> Login(LoginRequest request)
    {
        request.ValidateAndThrow();
        
        var tokenDto = await _service.Login(request.ToDto());

        return new ApiResponse<TokenDto>(true, tokenDto);
    }

    /*
    [HttpGet]
    public IActionResult Consult()
    {
        throw new NotImplementedException();
    }

    [HttpPatch]
    public IActionResult Update()
    {
        throw new NotImplementedException();
    }

    [HttpDelete]
    public IActionResult Deactivate()
    {
        throw new NotImplementedException();
    }


    [HttpPost("forgot-password")]
    public IActionResult ForgotPassword()
    {
        throw new NotImplementedException();
    }

    [HttpPost("reset-password")]
    public IActionResult ResetPassword()
    {
        throw new NotImplementedException();
    }
    */
}