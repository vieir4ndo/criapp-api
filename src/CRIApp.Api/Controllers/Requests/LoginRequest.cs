using CRIApp.Api.Controllers.Validators;
using CRIApp.Application.Dto;
using FluentValidation.Results;

namespace CRIApp.Api.Controllers.Requests;

public class LoginRequest : BaseRequest
{
    public string Username { get; set; }
    public string Password { get; set; }

    public LoginDto ToDto()
    {
        return new LoginDto()
        {
            Username = Username,
            Password = Password
        };
    }

    protected override ValidationResult Validate()
    {
        return  new LoginRequestValidator().Validate(this);
    }
}