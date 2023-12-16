using CRIApp.Api.Controllers.Validators;
using CRIApp.Application.Dtos;
using FluentValidation.Results;

namespace CRIApp.Api.Controllers.Requests;

public class ResetPasswordRequest : BaseRequest
{
    public string Username { get; set; }
    public string Code { get; set; }
    public string NewPassword { get; set; }
    
    protected override ValidationResult Validate()
    {
        return new ResetPasswordRequestValidator().Validate(this);
    }
    
    public ResetPasswordDto ToDto()
    {
        return new ResetPasswordDto
        {
            Username = Username,
            Code = Code,
            NewPassword = NewPassword
        };
    }
}