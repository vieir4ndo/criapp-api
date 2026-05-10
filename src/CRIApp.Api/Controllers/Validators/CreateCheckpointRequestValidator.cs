using CRIApp.Api.Controllers.Requests;
using FluentValidation;

namespace CRIApp.Api.Controllers.Validators;

public class CreateCheckpointRequestValidator : AbstractValidator<CreateCheckpointRequest>
{
    public CreateCheckpointRequestValidator()
    {
        RuleFor(x => x.Completed)
            .NotNull();
    }
}
