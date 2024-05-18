using CRIApp.Api.Controllers.Requests;
using FluentValidation;

namespace CRIApp.Api.Controllers.Validators;

public class UpdateForumRequestValidator : AbstractValidator<UpdateForumRequest>
{
    public UpdateForumRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .When(x => x.Name != null);
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .When(x => x.Description != null);
    }
}