using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Extensions;
using CRIApp.Domain.Settings;
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
            .When(_ => FeatureSettings.Current.PhoneNumberUsernameEnabled)
            .WithMessage(_ => ValidationMessages.UsernameEmailOrPhone);

        RuleFor(x => x.Username)
            .Must(ValidationExtensions.IsValidUsernameEmail)
            .When(_ => !FeatureSettings.Current.PhoneNumberUsernameEnabled)
            .WithMessage(_ => ValidationMessages.UsernameEmail);
    }
}