using System.ComponentModel.DataAnnotations;
using System.Globalization;
using CRIApp.Api.Controllers.Requests;
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
            .Must(BeValidIso8601Date)
            .WithMessage("BirthDate must be in format ISO 8601 (YYYY-MM-DD).");

        RuleFor(x => x.Sex)
            .NotEmpty()
            .NotNull()
            .IsInEnum();

        RuleFor(x => x.Username)
            .NotEmpty()
            .NotNull();
        
        RuleFor(x => x.Username)
            .Must(IsValidUsername)
            .When(_ => true);

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull();

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

    private static bool IsValidUsername(string username)
    {
        return new EmailAddressAttribute().IsValid(username) || new PhoneAttribute().IsValid(username);
    }

    private static bool BeValidIso8601Date(string date)
    {
        return DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }
}