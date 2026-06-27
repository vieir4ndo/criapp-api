namespace CRIApp.Application.Dtos;

public class TokenDto
{
    public required string Token { get; set; }
    public DateTime ExpiresIn { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiresIn { get; set; }
    public required UserDto User { get; set; }
}