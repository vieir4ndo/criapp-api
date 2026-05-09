using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Api.Tests.Requests;

public class CreateChildRequestTests
{
    private static CreateChildRequest ValidRequest() => new CreateChildRequest
    {
        Name = "Alice",
        BirthDate = "2020-03-10",
        Sex = Sex.Female,
        MonthsOfBreastFeeding = 6,
        IsPremature = false
    };

    [Fact]
    public void ValidRequest_ShouldNotThrow()
    {
        var ex = Record.Exception(() => ValidRequest().ValidateAndThrow());

        Assert.Null(ex);
    }

    [Fact]
    public void ValidRequest_ZeroMonths_ShouldNotThrow()
    {
        var request = ValidRequest();
        request.MonthsOfBreastFeeding = 0;

        var ex = Record.Exception(() => request.ValidateAndThrow());

        Assert.Null(ex);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void EmptyOrNullName_ShouldThrowValidationException(string? name)
    {
        var request = ValidRequest();
        request.Name = name!;

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }

    [Theory]
    [InlineData("10-03-2020")]
    [InlineData("2020/03/10")]
    [InlineData("not-a-date")]
    public void InvalidDateFormat_ShouldThrowValidationException(string date)
    {
        var request = ValidRequest();
        request.BirthDate = date;

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }

    [Fact]
    public void NegativeMonths_ShouldThrowValidationException()
    {
        var request = ValidRequest();
        request.MonthsOfBreastFeeding = -1;

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }
}
