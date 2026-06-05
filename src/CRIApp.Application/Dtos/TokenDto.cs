namespace CRIApp.Application.Dtos;

public class TokenDto
{
    public string Token { get; set; }
    public DateTime ExpiresIn { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiresIn { get; set; }
    public UserDto User { get; set; }
}