using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Api.Tests.Requests;

public class UpdateUserRequestTests
{
    [Fact]
    public void AllNullFields_ShouldNotThrow()
    {
        var request = new UpdateUserRequest();

        var ex = Record.Exception(() => request.ValidateAndThrow());

        Assert.Null(ex);
    }

    [Fact]
    public void ValidPartialUpdate_ShouldNotThrow()
    {
        var request = new UpdateUserRequest
        {
            Name = "New Name",
            BirthDate = "1992-08-20",
            Sex = Sex.Female
        };

        var ex = Record.Exception(() => request.ValidateAndThrow());

        Assert.Null(ex);
    }

    [Fact]
    public void EmptyName_WhenProvided_ShouldThrowValidationException()
    {
        var request = new UpdateUserRequest { Name = "" };

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }

    [Theory]
    [InlineData("20-08-1992")]
    [InlineData("1992/08/20")]
    public void InvalidDateFormat_WhenProvided_ShouldThrowValidationException(string date)
    {
        var request = new UpdateUserRequest { BirthDate = date };

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }

    [Fact]
    public void NegativeNumberOfChildren_WhenProvided_ShouldThrowValidationException()
    {
        var request = new UpdateUserRequest { NumberOfChildren = -1 };

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }
}
