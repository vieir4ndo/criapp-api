using CRIApp.Api.Controllers.Validators;
using CRIApp.Application.Dtos;
using FluentValidation.Results;

namespace CRIApp.Api.Controllers.Requests;

public class CreateAnswerRequest : BaseRequest
{
    public int Value { get; set; }
    public Guid QuestionId { get; set; }
    public Guid ChildId { get; set; }
    public Guid PrcId { get; set; }

    public CreateAnswerDto ToDto()
    {
        return new CreateAnswerDto()
        {
            Value = Value,
            QuestionId = QuestionId,
            ChildId = ChildId,
            PrcId = PrcId
        };
    }

    protected override ValidationResult Validate()
    {
        return new CreateAnswerRequestValidator().Validate(this);
    }
}