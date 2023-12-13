using System.Globalization;
using CRIApp.Api.Controllers.Requests;
using FluentValidation;

namespace CRIApp.Api.Controllers.Validators;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .When(x => x.Name != null);

        RuleFor(x => x.BirthDate)
            .Must(BeValidIso8601Date)
            .WithMessage("BirthDate must be in format ISO 8601 (YYYY-MM-DD).")
            .When(x => x.BirthDate != null);

        RuleFor(x => x.Sex)
            .NotEmpty()
            .IsInEnum()
            .When(x => x.Sex != null);;

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
    
    private static bool BeValidIso8601Date(string date)
    {
        return DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }
}