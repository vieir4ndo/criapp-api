using CRIApp.Api.Controllers.Requests;
using FluentValidation;

namespace CRIApp.Api.Controllers.Validators;

public class CreateForumRequestValidator : AbstractValidator<CreateForumRequest>
{
    public CreateForumRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .NotNull();
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .NotNull();
    }
}