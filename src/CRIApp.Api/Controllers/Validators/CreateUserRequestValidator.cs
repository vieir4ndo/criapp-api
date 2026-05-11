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
            .WithMessage(_ => ValidationMessages.BirthDateFormat);

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
            .WithMessage(_ => ValidationMessages.UsernameEmailOrPhone);

        RuleFor(x => x.Username)
            .Must(ValidationExtensions.IsValidUsernameEmail)
            .When(_ => !FeatureSettings.Current.PhoneNumberUsernameEnabled)
            .WithMessage(_ => ValidationMessages.UsernameEmail);

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.Password)
            .Must(ValidationExtensions.IsStrongPassword)
            .WithMessage(_ => ValidationMessages.PasswordStrength)
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