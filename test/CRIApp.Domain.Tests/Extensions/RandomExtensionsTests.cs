using CRIApp.Domain.Extensions;
using Xunit;

namespace CRIApp.Domain.Tests.Extensions;

public class RandomExtensionsTests
{
    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(1)]
    public void GenerateRandomString_ShouldReturnCorrectLength(int length)
    {
        var result = RandomExtensions.GenerateRandomString(length);

        Assert.Equal(length, result.Length);
    }

    [Fact]
    public void GenerateRandomString_ShouldReturnOnlyAlphanumericCharacters()
    {
        var result = RandomExtensions.GenerateRandomString(100);

        Assert.Matches("^[A-Za-z0-9]+$", result);
    }

    [Fact]
    public void GenerateRandomString_TwoCalls_ShouldReturnDifferentValues()
    {
        var result1 = RandomExtensions.GenerateRandomString(20);
        var result2 = RandomExtensions.GenerateRandomString(20);

        Assert.NotEqual(result1, result2);
    }
}
