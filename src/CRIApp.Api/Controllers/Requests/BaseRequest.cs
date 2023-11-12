
using CRIApp.Domain.Exceptions;
using FluentValidation.Results;

namespace CRIApp.Api.Controllers.Requests;

public abstract class BaseRequest
{
     public void ValidateAndThrow()
     {
         var result = Validate();

        if (result.IsValid) 
            return;
        
        var errors = new List<string>();
        result.Errors.ForEach(x => errors.Add(x.ErrorMessage));
        throw new ValidationException(errors);
    }

     protected abstract ValidationResult Validate();
}