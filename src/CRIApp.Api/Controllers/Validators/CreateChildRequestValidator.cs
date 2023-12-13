using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Extensions;
using FluentValidation;

namespace CRIApp.Api.Controllers.Validators;

public class CreateChildRequestValidator : AbstractValidator<CreateChildRequest>
{
    public CreateChildRequestValidator()
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

        RuleFor(x => x.MonthsOfBreastFeeding)
            .GreaterThanOrEqualTo(0)
            .NotNull();

        RuleFor(x => x.IsPremature)
            .NotNull();
    }
}