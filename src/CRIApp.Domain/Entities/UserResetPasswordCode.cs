using CRIApp.Domain.Extensions;

namespace CRIApp.Domain.Entities;

public class UserResetPasswordCode : Entity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public string CodeHash { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private UserResetPasswordCode()
    {
        
    }

    public UserResetPasswordCode(Guid userId, string code, DateTime expiresAt)
    {
        UserId = userId;
        CodeHash = HashExtensions.HashPassword(code);
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.Now.ToUniversalTime();
    }
}