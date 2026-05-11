using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using CRIApp.Domain.Settings;
using Xunit;

namespace CRIApp.Api.Tests.Requests;

public class CreateUserRequestTests
{
    public CreateUserRequestTests()
    {
        FeatureSettings.Initialize(new FeatureSettings { PhoneNumberUsernameEnabled = true });
    }

    private static CreateUserRequest ValidRequest() => new CreateUserRequest
    {
        Name = "Test User",
        BirthDate = "1990-06-15",
        Sex = Sex.Male,
        Username = "user@domain.com",
        Password = "Password1!"
    };

    [Fact]
    public void ValidRequest_ShouldNotThrow()
    {
        var ex = Record.Exception(() => ValidRequest().ValidateAndThrow());

        Assert.Null(ex);
    }

    [Fact]
    public void ValidRequest_WithPhoneUsername_ShouldNotThrow()
    {
        var request = ValidRequest();
        request.Username = "+34612345678";

        var ex = Record.Exception(() => request.ValidateAndThrow());

        Assert.Null(ex);
    }

    [Fact]
    public void ValidRequest_WithOptionalFields_ShouldNotThrow()
    {
        var request = ValidRequest();
        request.FamilyType = FamilyType.HeterosexualCouple;
        request.EducationalLevel = EducationalLevel.University;
        request.NumberOfChildren = 2;

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
    [InlineData("15-06-1990")]
    [InlineData("1990/06/15")]
    [InlineData("not-a-date")]
    public void InvalidDateFormat_ShouldThrowValidationException(string date)
    {
        var request = ValidRequest();
        request.BirthDate = date;

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void EmptyOrNullPassword_ShouldThrowValidationException(string? password)
    {
        var request = ValidRequest();
        request.Password = password!;

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }

    [Theory]
    [InlineData("not-an-email-or-phone")]
    [InlineData("invalid")]
    public void InvalidUsername_ShouldThrowValidationException(string username)
    {
        var request = ValidRequest();
        request.Username = username;

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }

    [Fact]
    public void NegativeNumberOfChildren_ShouldThrowValidationException()
    {
        var request = ValidRequest();
        request.NumberOfChildren = -1;

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }
}
