namespace CRIApp.Application.Abstractions;

public interface IMessage
{
    Task SendEmail(string email, string message);
    void SendSms(string phoneNumber, string message);
}