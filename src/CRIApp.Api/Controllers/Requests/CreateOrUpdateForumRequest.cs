using CRIApp.Api.Controllers.Validators;
using CRIApp.Application.Dtos;
using FluentValidation.Results;

namespace CRIApp.Api.Controllers.Requests;

public class CreateOrUpdateForumRequest : BaseRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    protected override ValidationResult Validate()
    {
        return new CreateOrUpdateForumRequestValidator().Validate(this);
    }
    
    public CreateOrUpdateForumDto ToDto(Guid prcId)
    {
        return new CreateOrUpdateForumDto
        {
            Name = Name,
            Description = Description,
            PrcId = prcId
        };
    }
}