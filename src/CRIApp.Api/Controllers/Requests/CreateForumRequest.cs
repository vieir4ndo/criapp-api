using CRIApp.Api.Controllers.Validators;
using CRIApp.Application.Dtos;
using FluentValidation.Results;

namespace CRIApp.Api.Controllers.Requests;

public class CreateForumRequest : BaseRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    protected override ValidationResult Validate()
    {
        return new CreateForumRequestValidator().Validate(this);
    }
    
    public CreateForumDto ToDto(Guid prcId)
    {
        return new CreateForumDto
        {
            Name = Name,
            Description = Description,
            PrcId = prcId
        };
    }
}