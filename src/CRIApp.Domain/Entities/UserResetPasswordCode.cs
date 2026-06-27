using CRIApp.Domain.Extensions;

namespace CRIApp.Domain.Entities;

public class UserResetPasswordCode : Entity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string CodeHash { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public UserResetPasswordCode(Guid userId, string code, DateTime expiresAt)
    {
        code.IsNullOrEmpty("A reset password code cannot be null or empty");

        UserId = userId;
        CodeHash = HashExtensions.HashPassword(code);
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.Now.ToUniversalTime();

        Validate();
    }

    private void Validate()
    {
        UserId.IsEmptyGuid("A reset password code has to have a user associated");
        ExpiresAt.IsDateTimeMinValue("A reset password code has to have a valid expiry date");
    }

    private UserResetPasswordCode() { }
}