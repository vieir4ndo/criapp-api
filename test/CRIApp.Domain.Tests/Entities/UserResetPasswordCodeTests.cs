using CRIApp.Domain.Entities;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Domain.Tests.Entities;

public class UserResetPasswordCodeTests
{
    [Fact]
    public void Create_ValidCode_ShouldHashCodeAndSetExpiry()
    {
        var userId = Guid.NewGuid();
        var expiresAt = DateTime.UtcNow.AddSeconds(180);

        var code = new UserResetPasswordCode(userId, "ABC12", expiresAt);

        Assert.Equal(userId, code.UserId);
        Assert.NotEqual("ABC12", code.CodeHash);
        Assert.Equal(expiresAt, code.ExpiresAt);
    }

    [Fact]
    public void Create_ShouldSetCreatedAtToUtcNow()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);

        var code = new UserResetPasswordCode(Guid.NewGuid(), "XYZ99", DateTime.UtcNow.AddSeconds(180));

        Assert.True(code.CreatedAt >= before);
        Assert.True(code.CreatedAt <= DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void Create_DifferentCodes_ShouldProduceDifferentHashes()
    {
        var userId = Guid.NewGuid();
        var expiry = DateTime.UtcNow.AddSeconds(180);

        var code1 = new UserResetPasswordCode(userId, "CODE1", expiry);
        var code2 = new UserResetPasswordCode(userId, "CODE2", expiry);

        Assert.NotEqual(code1.CodeHash, code2.CodeHash);
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new UserResetPasswordCode(Guid.Empty, "ABC12", DateTime.UtcNow.AddSeconds(180)));
    }

    [Fact]
    public void Create_WithEmptyCode_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new UserResetPasswordCode(Guid.NewGuid(), "", DateTime.UtcNow.AddSeconds(180)));
    }

    [Fact]
    public void Create_WithNullCode_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new UserResetPasswordCode(Guid.NewGuid(), null!, DateTime.UtcNow.AddSeconds(180)));
    }

    [Fact]
    public void Create_WithMinValueExpiresAt_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new UserResetPasswordCode(Guid.NewGuid(), "ABC12", DateTime.MinValue));
    }
}
