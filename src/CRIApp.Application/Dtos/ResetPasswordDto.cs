namespace CRIApp.Application.Dtos;

public class ResetPasswordDto
{
    public required string Username { get; set; }
    public required string Code { get; set; }
    public required string NewPassword { get; set; }
}