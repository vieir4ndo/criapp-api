using CRIApp.Api.Controllers.Requests;
using FluentValidation;

namespace CRIApp.Api.Controllers.Validators;

public class UpdateForumRequestValidator : AbstractValidator<UpdateForumRequest>
{
    public UpdateForumRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .NotNull()
            .When(x => x != null);
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .NotNull()
            .When(x => x != null);
    }
}