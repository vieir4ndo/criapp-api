using CRIApp.Api.Controllers.Validators;
using CRIApp.Application.Dtos;
using FluentValidation.Results;

namespace CRIApp.Api.Controllers.Requests;

public class UpdateForumRequest : BaseRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    protected override ValidationResult Validate()
    {
        return new UpdateForumRequestValidator().Validate(this);
    }
    
    public UpdateForumDto ToDto(Guid prcId)
    {
        return new UpdateForumDto
        {
            Name = Name,
            Description = Description,
            PrcId = prcId
        };
    }
}