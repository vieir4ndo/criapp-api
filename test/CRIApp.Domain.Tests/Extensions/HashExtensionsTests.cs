using CRIApp.Domain.Extensions;
using Xunit;

namespace CRIApp.Domain.Tests.Extensions;

public class HashExtensionsTests
{
    [Fact]
    public void HashPassword_ShouldReturnNonEmptyHash()
    {
        var hash = HashExtensions.HashPassword("mypassword");

        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }

    [Fact]
    public void HashPassword_SameInput_ShouldReturnSameHash()
    {
        var hash1 = HashExtensions.HashPassword("mypassword");
        var hash2 = HashExtensions.HashPassword("mypassword");

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void HashPassword_DifferentInputs_ShouldReturnDifferentHashes()
    {
        var hash1 = HashExtensions.HashPassword("password1");
        var hash2 = HashExtensions.HashPassword("password2");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void HashPassword_ShouldReturn64CharHexString()
    {
        var hash = HashExtensions.HashPassword("mypassword");

        Assert.Equal(64, hash.Length);
        Assert.Matches("^[0-9a-f]+$", hash);
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ShouldReturnTrue()
    {
        var hash = HashExtensions.HashPassword("correctpassword");

        Assert.True(HashExtensions.VerifyPassword("correctpassword", hash));
    }

    [Fact]
    public void VerifyPassword_WrongPassword_ShouldReturnFalse()
    {
        var hash = HashExtensions.HashPassword("correctpassword");

        Assert.False(HashExtensions.VerifyPassword("wrongpassword", hash));
    }

    [Fact]
    public void VerifyPassword_CaseSensitive_ShouldReturnFalse()
    {
        var hash = HashExtensions.HashPassword("Password");

        Assert.False(HashExtensions.VerifyPassword("password", hash));
    }
}
