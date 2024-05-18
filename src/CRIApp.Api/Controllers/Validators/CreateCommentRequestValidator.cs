using CRIApp.Api.Controllers.Requests;
using FluentValidation;

namespace CRIApp.Api.Controllers.Validators;

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .NotNull();
        
        RuleFor(x => x.ForumId)
            .NotEmpty()
            .NotNull();
    }
}