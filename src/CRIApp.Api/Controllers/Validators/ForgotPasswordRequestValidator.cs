using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Extensions;
using FluentValidation;

namespace CRIApp.Api.Controllers.Validators;

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .NotNull();
        
        RuleFor(x => x.Username)
            .Must(ValidationExtensions.IsValidUsername)
            .When(_ => true);
    }
}