namespace CRIApp.Domain.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(List<string> errors) : base(errors)
    {
    }

    public NotFoundException(string error) : base(error)
    {
    }
}