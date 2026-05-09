using CRIApp.Domain.Extensions;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Domain.Tests.Extensions;

public class ValidationExtensionsTests
{
    [Fact]
    public void IsNullOrEmpty_WithNullString_ShouldThrowDomainException()
    {
        string? text = null;

        Assert.Throws<DomainException>(() => text!.IsNullOrEmpty("error"));
    }

    [Fact]
    public void IsNullOrEmpty_WithEmptyString_ShouldThrowDomainException()
    {
        var text = string.Empty;

        Assert.Throws<DomainException>(() => text.IsNullOrEmpty("error"));
    }

    [Fact]
    public void IsNullOrEmpty_WithValidString_ShouldNotThrow()
    {
        var text = "valid";

        var ex = Record.Exception(() => text.IsNullOrEmpty("error"));

        Assert.Null(ex);
    }

    [Fact]
    public void IsEmptyGuid_WithEmptyGuid_ShouldThrowDomainException()
    {
        var guid = Guid.Empty;

        Assert.Throws<DomainException>(() => guid.IsEmptyGuid("error"));
    }

    [Fact]
    public void IsEmptyGuid_WithValidGuid_ShouldNotThrow()
    {
        var guid = Guid.NewGuid();

        var ex = Record.Exception(() => guid.IsEmptyGuid("error"));

        Assert.Null(ex);
    }

    [Fact]
    public void IsActive_WithFalse_ShouldThrowDomainException()
    {
        var active = false;

        Assert.Throws<DomainException>(() => active.IsActive("error"));
    }

    [Fact]
    public void IsActive_WithTrue_ShouldNotThrow()
    {
        var active = true;

        var ex = Record.Exception(() => active.IsActive("error"));

        Assert.Null(ex);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(6)]
    public void IsValueInRange_OutOfRange_ShouldThrowDomainException(int value)
    {
        Assert.Throws<DomainException>(() => value.IsValueInRange(0, 5, "error"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void IsValueInRange_InRange_ShouldNotThrow(int value)
    {
        var ex = Record.Exception(() => value.IsValueInRange(0, 5, "error"));

        Assert.Null(ex);
    }

    [Fact]
    public void IsDateTimeMinValue_WithMinValue_ShouldThrowDomainException()
    {
        var date = DateTime.MinValue;

        Assert.Throws<DomainException>(() => date.IsDateTimeMinValue("error"));
    }

    [Fact]
    public void IsDateTimeMinValue_WithValidDate_ShouldNotThrow()
    {
        var date = new DateTime(1990, 1, 1);

        var ex = Record.Exception(() => date.IsDateTimeMinValue("error"));

        Assert.Null(ex);
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("+34612345678")]
    public void IsValidUsername_WithValidUsername_ShouldNotThrow(string username)
    {
        var ex = Record.Exception(() => username.IsValidUsername("error"));

        Assert.Null(ex);
    }

    [Theory]
    [InlineData("not-an-email-or-phone")]
    [InlineData("invalid")]
    public void IsValidUsername_WithInvalidUsername_ShouldThrowDomainException(string username)
    {
        Assert.Throws<DomainException>(() => username.IsValidUsername("error"));
    }

    [Fact]
    public void HasElements_WithNonEmptyList_ShouldThrowDomainException()
    {
        var list = new List<int> { 1, 2, 3 };

        Assert.Throws<DomainException>(() => list.HasElements("error"));
    }

    [Fact]
    public void HasElements_WithEmptyList_ShouldNotThrow()
    {
        var list = new List<int>();

        var ex = Record.Exception(() => list.HasElements("error"));

        Assert.Null(ex);
    }

    [Fact]
    public void IsPositive_WithNegativeNumber_ShouldThrowDomainException()
    {
        var number = -1;

        Assert.Throws<DomainException>(() => number.IsPositive("error"));
    }

    [Fact]
    public void IsPositive_WithZero_ShouldNotThrow()
    {
        var number = 0;

        var ex = Record.Exception(() => number.IsPositive("error"));

        Assert.Null(ex);
    }

    [Fact]
    public void IsDateOnlyMinValue_WithMinValue_ShouldThrowDomainException()
    {
        var date = DateOnly.MinValue;

        Assert.Throws<DomainException>(() => date.IsDateOnlyMinValue("error"));
    }

    [Fact]
    public void IsDateOnlyMinValue_WithValidDate_ShouldNotThrow()
    {
        var date = new DateOnly(2020, 1, 1);

        var ex = Record.Exception(() => date.IsDateOnlyMinValue("error"));

        Assert.Null(ex);
    }

    [Theory]
    [InlineData("2020-01-15")]
    [InlineData("1990-12-31")]
    public void BeValidIso8601Date_WithValidDate_ShouldReturnTrue(string date)
    {
        Assert.True(ValidationExtensions.BeValidIso8601Date(date));
    }

    [Theory]
    [InlineData("15-01-2020")]
    [InlineData("2020/01/15")]
    [InlineData("not-a-date")]
    [InlineData(null)]
    public void BeValidIso8601Date_WithInvalidDate_ShouldReturnFalse(string? date)
    {
        Assert.False(ValidationExtensions.BeValidIso8601Date(date));
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("+34612345678")]
    public void IsValidUsername_Static_WithValidUsername_ShouldReturnTrue(string username)
    {
        Assert.True(ValidationExtensions.IsValidUsername(username));
    }

    [Theory]
    [InlineData("not-valid")]
    [InlineData("no_at")]
    public void IsValidUsername_Static_WithInvalidUsername_ShouldReturnFalse(string username)
    {
        Assert.False(ValidationExtensions.IsValidUsername(username));
    }
}
