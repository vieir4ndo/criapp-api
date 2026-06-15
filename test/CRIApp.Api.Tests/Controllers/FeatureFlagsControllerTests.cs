using CRIApp.Api.Controllers;
using CRIApp.Api.Controllers.Responses;
using CRIApp.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CRIApp.Api.Tests.Controllers;

public class FeatureFlagsControllerTests
{
    private static FeatureFlagsController CreateSut(bool phoneNumberUsernameEnabled)
    {
        var settings = Options.Create(new FeatureSettings { PhoneNumberUsernameEnabled = phoneNumberUsernameEnabled });
        var logger = new Mock<ILogger<FeatureFlagsController>>();
        return new FeatureFlagsController(settings, logger.Object);
    }

    [Fact]
    public void Get_PhoneNumberEnabled_ShouldReturnTrue()
    {
        var sut = CreateSut(phoneNumberUsernameEnabled: true);

        var result = sut.Get();

        Assert.True(result.Success);
        Assert.True(result.Data.Single().PhoneNumberUsernameEnabled);
    }

    [Fact]
    public void Get_PhoneNumberDisabled_ShouldReturnFalse()
    {
        var sut = CreateSut(phoneNumberUsernameEnabled: false);

        var result = sut.Get();

        Assert.True(result.Success);
        Assert.False(result.Data.Single().PhoneNumberUsernameEnabled);
    }
}
