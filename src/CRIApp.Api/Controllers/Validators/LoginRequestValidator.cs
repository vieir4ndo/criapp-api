using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Extensions;
using CRIApp.Domain.Settings;
using FluentValidation;

namespace CRIApp.Api.Controllers.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .NotNull();
        
        RuleFor(x => x.Username)
            .Must(ValidationExtensions.IsValidUsername)
            .When(_ => FeatureSettings.Current.PhoneNumberUsernameEnabled)
            .WithMessage("Username must be a valid email address or phone number.");

        RuleFor(x => x.Username)
            .Must(ValidationExtensions.IsValidUsernameEmail)
            .When(_ => !FeatureSettings.Current.PhoneNumberUsernameEnabled)
            .WithMessage("Username must be a valid email address.");
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull();
    }
}