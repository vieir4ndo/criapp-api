using CRIApp.Api.Controllers.Requests;
using FluentValidation;

namespace CRIApp.Api.Controllers.Validators;

public class CreateOrUpdateForumRequestValidator : AbstractValidator<CreateOrUpdateForumRequest>
{
    public CreateOrUpdateForumRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .NotNull();
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .NotNull();
    }
}