using CRIApp.Api.Controllers.Validators;
using FluentValidation.Results;

namespace CRIApp.Api.Controllers.Requests;

public class ForgotPasswordRequest : BaseRequest
{
    public string Username { get; set; }
    
    protected override ValidationResult Validate()
    {
        return new ForgotPasswordRequestValidator().Validate(this);
    }
}