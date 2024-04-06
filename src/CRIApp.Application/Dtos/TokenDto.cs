namespace CRIApp.Application.Dtos;

public class TokenDto
{
    public string Token { get; set; }
    public DateTime ExpiresIn { get; set; }
    public UserDto User { get; set; }
}