namespace CRIApp.Application.Abstractions;

public interface IEmailClient
{
    Task SendAsync(string from, string to, string subject, string body);
}
