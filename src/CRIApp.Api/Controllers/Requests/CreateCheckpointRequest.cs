using CRIApp.Api.Controllers.Validators;
using FluentValidation.Results;

namespace CRIApp.Api.Controllers.Requests;

public class CreateCheckpointRequest : BaseRequest
{
    public bool Completed { get; set; }

    protected override ValidationResult Validate()
    {
        return new CreateCheckpointRequestValidator().Validate(this);
    }
}
