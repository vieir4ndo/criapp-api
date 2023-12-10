using CRIApp.Api.Controllers.Validators;
using FluentValidation.Results;

namespace CRIApp.Api.Controllers.Requests;

public class GetQuestionsToReplyRequest : BaseRequest
{
    public Guid ChildId { get; set; }
    protected override ValidationResult Validate()
    {
        return new GetQuestionsToReplyRequestValidator().Validate(this);
    }
}