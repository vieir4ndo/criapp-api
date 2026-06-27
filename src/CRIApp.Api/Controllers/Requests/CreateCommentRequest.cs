using CRIApp.Api.Controllers.Validators;
using CRIApp.Application.Dtos;
using FluentValidation.Results;

namespace CRIApp.Api.Controllers.Requests;

public class CreateCommentRequest : BaseRequest
{
    public required string Text { get; set; }
    public required Guid ForumId { get; set; }
    
    protected override ValidationResult Validate()
    {
        return new CreateCommentRequestValidator().Validate(this);
    }

    public CreateCommentDto ToDto(Guid userId)
    {
        return new CreateCommentDto
        {
            Text = Text,
            PrcId = userId,
            ForumId = ForumId
        };
    }
}