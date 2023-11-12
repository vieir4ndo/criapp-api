namespace CRIApp.Domain.Exceptions;

public class DomainException : BaseException
{
    public DomainException(List<string> errors) : base(errors)
    {
    }

    public DomainException(string error) : base(error)
    {
    }
}