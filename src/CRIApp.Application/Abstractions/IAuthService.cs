using CRIApp.Application.Dtos;

namespace CRIApp.Application.Abstractions;

public interface IAuthService
{
    Task<TokenDto> Login(LoginDto dto);
    Task<TokenDto> RefreshToken(string refreshToken);
    Task ForgotPassword(string username);
    Task ResetPassword(ResetPasswordDto dto);
}
