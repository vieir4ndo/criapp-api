namespace CRIApp.Domain.Exceptions;

public class ValidationException : BaseException
{
    public ValidationException(List<string> errors) : base(errors)
    {
    }

    public ValidationException(string error) : base(error)
    {
    }
}