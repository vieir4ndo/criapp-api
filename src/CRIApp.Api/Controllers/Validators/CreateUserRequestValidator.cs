using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Extensions;
using CRIApp.Domain.Settings;
using FluentValidation;

namespace CRIApp.Api.Controllers.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.BirthDate)
            .Must(ValidationExtensions.BeValidIso8601Date)
            .WithMessage("BirthDate must be in format ISO 8601 (YYYY-MM-DD).");

        RuleFor(x => x.Sex)
            .NotEmpty()
            .NotNull()
            .IsInEnum();

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

        RuleFor(x => x.Password)
            .Must(ValidationExtensions.IsStrongPassword)
            .WithMessage("Password must be at least 8 characters and contain uppercase, lowercase, digit and special character.")
            .When(x => !string.IsNullOrEmpty(x.Password));

        RuleFor(x => x.FamilyType)
            .IsInEnum()
            .When(x => x != null);

        RuleFor(x => x.EducationalLevel)
            .IsInEnum()
            .When(x => x != null);
        
        RuleFor(x => x.NumberOfChildren)
            .GreaterThanOrEqualTo(0)
            .When(x => x != null);
    }
}