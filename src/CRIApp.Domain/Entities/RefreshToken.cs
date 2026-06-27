using CRIApp.Domain.Extensions;

namespace CRIApp.Domain.Entities;

public class RefreshToken : Entity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string TokenHash { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public RefreshToken(Guid userId, string rawToken, DateTime expiresAt)
    {
        rawToken.IsNullOrEmpty("A refresh token cannot be null or empty");

        UserId = userId;
        TokenHash = HashExtensions.HashPassword(rawToken);
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    private void Validate()
    {
        UserId.IsEmptyGuid("A refresh token has to have a user associated");
        ExpiresAt.IsDateTimeMinValue("A refresh token has to have a valid expiry date");
    }

    private RefreshToken() { }

    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;
}
