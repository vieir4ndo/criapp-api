using CRIApp.Api.Controllers.Validators;
using FluentValidation.Results;

namespace CRIApp.Api.Controllers.Requests;

public class RefreshTokenRequest : BaseRequest
{
    public required string RefreshToken { get; set; }

    protected override ValidationResult Validate()
    {
        return new RefreshTokenRequestValidator().Validate(this);
    }
}
