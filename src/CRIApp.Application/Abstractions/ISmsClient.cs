namespace CRIApp.Application.Abstractions;

public interface ISmsClient
{
    void Send(string to, string from, string body);
}
