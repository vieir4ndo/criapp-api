using CRIApp.Domain.Entities;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Domain.Tests.Entities;

public class RefreshTokenTests
{
    [Fact]
    public void Create_ValidToken_ShouldHashTokenAndSetProperties()
    {
        var userId = Guid.NewGuid();
        var expiresAt = DateTime.UtcNow.AddDays(7);

        var token = new RefreshToken(userId, "raw-token-value", expiresAt);

        Assert.Equal(userId, token.UserId);
        Assert.NotEqual("raw-token-value", token.TokenHash);
        Assert.Equal(expiresAt, token.ExpiresAt);
    }

    [Fact]
    public void Create_ShouldSetCreatedAtToUtcNow()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);

        var token = new RefreshToken(Guid.NewGuid(), "raw-token", DateTime.UtcNow.AddDays(7));

        Assert.True(token.CreatedAt >= before);
        Assert.True(token.CreatedAt <= DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void Create_DifferentTokens_ShouldProduceDifferentHashes()
    {
        var userId = Guid.NewGuid();
        var expiry = DateTime.UtcNow.AddDays(7);

        var token1 = new RefreshToken(userId, "token-a", expiry);
        var token2 = new RefreshToken(userId, "token-b", expiry);

        Assert.NotEqual(token1.TokenHash, token2.TokenHash);
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new RefreshToken(Guid.Empty, "raw-token", DateTime.UtcNow.AddDays(7)));
    }

    [Fact]
    public void Create_WithEmptyToken_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new RefreshToken(Guid.NewGuid(), "", DateTime.UtcNow.AddDays(7)));
    }

    [Fact]
    public void Create_WithNullToken_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new RefreshToken(Guid.NewGuid(), null!, DateTime.UtcNow.AddDays(7)));
    }

    [Fact]
    public void Create_WithMinValueExpiresAt_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new RefreshToken(Guid.NewGuid(), "raw-token", DateTime.MinValue));
    }

    [Fact]
    public void IsExpired_FutureExpiry_ShouldReturnFalse()
    {
        var token = new RefreshToken(Guid.NewGuid(), "raw-token", DateTime.UtcNow.AddDays(7));

        Assert.False(token.IsExpired());
    }

    [Fact]
    public void IsExpired_PastExpiry_ShouldReturnTrue()
    {
        var token = new RefreshToken(Guid.NewGuid(), "raw-token", DateTime.UtcNow.AddSeconds(1));

        System.Threading.Thread.Sleep(1100);

        Assert.True(token.IsExpired());
    }
}
