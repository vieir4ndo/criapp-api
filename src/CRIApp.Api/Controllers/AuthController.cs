using CRIApp.Api.Controllers.Requests;
using CRIApp.Api.Controllers.Responses;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CRIApp.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("auth")]
public class AuthController : CriAppControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service, ILogger<AuthController> logger) : base(logger)
    {
        _service = service;
    }

    [HttpPost("login")]
    public async Task<ApiResponse<TokenDto>> Login(LoginRequest request)
    {
        ValidateAndLog(request);

        var tokenDto = await _service.Login(request.ToDto());

        return new ApiResponse<TokenDto>(true, tokenDto);
    }

    [HttpPost("refresh-token")]
    public async Task<ApiResponse<TokenDto>> RefreshToken(RefreshTokenRequest request)
    {
        ValidateAndLog(request);

        var tokenDto = await _service.RefreshToken(request.RefreshToken);

        return new ApiResponse<TokenDto>(true, tokenDto);
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
}
