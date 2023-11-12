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
            .NotEmpty()
            .NotNull()
            .GreaterThan(DateTime.MinValue);

        RuleFor(x => x.Sex)
            .NotEmpty()
            .NotNull()
            .IsInEnum();

        RuleFor(x => x.Email)
            .NotEmpty()
            .NotNull()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull();
        
        RuleFor(x => x.FamilyType)
            .IsInEnum()
            .When(x => x != null);
        
        RuleFor(x => x.EducationalLevel)
            .IsInEnum()
            .When(x => x != null);
    }
}