namespace CRIApp.Domain.Exceptions;

public abstract class BaseException : Exception
{
    public List<string> Errors { get;  private set; }
    
    public BaseException(List<string> errors)
    {
        Errors = errors;
    }

    public BaseException(string error) : base(error)
    {
        Errors = new List<string>() { error };
    }
}