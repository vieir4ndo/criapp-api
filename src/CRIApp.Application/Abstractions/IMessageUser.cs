using CRIApp.Domain.Entities;

namespace CRIApp.Application.Abstractions;

public interface IMessageUser
{
    Task NotifyAccountCreation(User user);
    Task NotifyForgotPassword(User user, string code);
}