using CRIApp.Domain.Settings;
using Xunit;

namespace CRIApp.Domain.Tests.Settings;

public class SettingsTests
{
    [Fact]
    public void JwtTokenSettings_SectionName_ShouldBeExpectedValue()
    {
        Assert.Equal("JwtTokenSettings", JwtTokenSettings.SectionName);
    }

    [Fact]
    public void JwtTokenSettings_Properties_ShouldBeSettable()
    {
        var settings = new JwtTokenSettings
        {
            Key = "supersecretkey",
            Issuer = "criapp",
            ExpiresInMinutes = 5
        };

        Assert.Equal("supersecretkey", settings.Key);
        Assert.Equal("criapp", settings.Issuer);
        Assert.Equal(5, settings.ExpiresInMinutes);
    }
}
