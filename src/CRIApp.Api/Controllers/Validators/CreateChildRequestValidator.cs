using CRIApp.Api.Controllers.Requests;
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
            .NotNull()
            .NotEmpty()
            .GreaterThan(DateTime.MinValue);
        
        RuleFor(x => x.Sex)
            .NotEmpty()
            .NotNull()
            .IsInEnum();

        RuleFor(x => x.MonthsOfBreastFeeding)
            .GreaterThan(0)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.PrcId)
            .NotNull()
            .NotEmpty();
    }
}