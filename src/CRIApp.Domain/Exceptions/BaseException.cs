namespace CRIApp.Domain.Exceptions;

public abstract class BaseException : Exception
{
    public List<string> Errors { get;  private set; }

    protected BaseException(List<string> errors)
    {
        Errors = errors;
    }

    protected BaseException(string error) : base(error)
    {
        Errors = new List<string>() { error };
    }
}