using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Extensions;
using FluentValidation;

namespace CRIApp.Api.Controllers.Validators;

public class UpdateChildRequestValidator : AbstractValidator<UpdateChildRequest>
{
    public UpdateChildRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .When(x => x.Name != null);

        RuleFor(x => x.BirthDate)
            .Must(ValidationExtensions.BeValidIso8601Date)
            .WithMessage(_ => ValidationMessages.BirthDateFormat)
            .When(x => x.BirthDate != null);
        
        RuleFor(x => x.Sex)
            .NotEmpty()
            .IsInEnum()
            .When(x => x.Sex != null);

        RuleFor(x => x.MonthsOfBreastFeeding)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MonthsOfBreastFeeding != null);
    }
}