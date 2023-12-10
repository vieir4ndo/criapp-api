using CRIApp.Api.Controllers.Requests;
using FluentValidation;

namespace CRIApp.Api.Controllers.Validators;

public class GetQuestionsToReplyRequestValidator : AbstractValidator<GetQuestionsToReplyRequest>
{
    public GetQuestionsToReplyRequestValidator()
    {
        RuleFor(x => x.ChildId).NotEmpty().NotNull();
    }
}