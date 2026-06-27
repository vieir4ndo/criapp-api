namespace CRIApp.Application.Abstractions;

public interface ISmsClient
{
    Task SendAsync(string to, string from, string body);
}
