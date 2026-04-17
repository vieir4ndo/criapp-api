using CRIApp.Api.Controllers.Requests;
using CRIApp.Api.Controllers.Responses;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CRIApp.Api.Controllers;

[ApiController]
[Route("user")]
public class UserController : CriAppControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ApiResponse<object>> Create(CreateUserRequest request)
    {
        ValidateAndLog(request);

        await _service.Create(request.ToDto());

        return new ApiResponse<object>(true);
    }

    [HttpPost("login")]
    public async Task<ApiResponse<TokenDto>> Login(LoginRequest request)
    {
        ValidateAndLog(request);

        var tokenDto = await _service.Login(request.ToDto());

        return new ApiResponse<TokenDto>(true, tokenDto);
    }

    [HttpGet]
    public async Task<ApiResponse<UserDto>> Get()
    {
        var user = await _service.GetAndValidateById(GetUserId());

        return new ApiResponse<UserDto>(true, new UserDto(user));
    }
    
    [HttpPatch]
    public async Task<ApiResponse<object>> Update(UpdateUserRequest request)
    {
        ValidateAndLog(request);

        await _service.Update(GetUserId(), request.ToDto());

        return new ApiResponse<object>(true);
    }
    
    [HttpPost("forgot-password")]
    public async Task<ApiResponse<object>> ForgotPassword(ForgotPasswordRequest request)
    {
        ValidateAndLog(request);
        
        await _service.ForgotPassword(request.Username);
        
        return new ApiResponse<object>(true);
    }

    [HttpPost("reset-password")]
    public async Task<ApiResponse<object>> ResetPassword(ResetPasswordRequest request)
    {
        ValidateAndLog(request);

        await _service.ResetPassword(request.ToDto());

        return new ApiResponse<object>(true);
    }

    /*
[HttpDelete]
public IActionResult Deactivate()
{
    throw new NotImplementedException();
}



*/
}