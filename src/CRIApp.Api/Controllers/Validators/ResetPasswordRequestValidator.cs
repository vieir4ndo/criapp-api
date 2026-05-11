using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Extensions;
using CRIApp.Domain.Settings;
using FluentValidation;

namespace CRIApp.Api.Controllers.Validators;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
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
        
        RuleFor(x => x.Code)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.NewPassword)
            .Must(ValidationExtensions.IsStrongPassword)
            .WithMessage("Password must be at least 8 characters and contain uppercase, lowercase, digit and special character.")
            .When(x => !string.IsNullOrEmpty(x.NewPassword));
    }
}