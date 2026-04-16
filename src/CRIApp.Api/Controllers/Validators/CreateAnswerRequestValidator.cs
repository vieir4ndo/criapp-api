using CRIApp.Api.Controllers.Requests;
using FluentValidation;

namespace CRIApp.Api.Controllers.Validators;

public class CreateAnswerRequestValidator : AbstractValidator<CreateAnswerRequest>
{
    public CreateAnswerRequestValidator()
    {
        RuleFor(x => x.Value)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(5);
        
        RuleFor(x => x.QuestionId)
            .NotEmpty()
            .NotNull();
        
        RuleFor(x => x.ChildId)
            .NotEmpty()
            .NotNull();
    }
}