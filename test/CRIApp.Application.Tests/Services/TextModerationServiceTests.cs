using CRIApp.Application.Services;
using Xunit;

namespace CRIApp.Application.Tests.Services;

public class TextModerationServiceTests
{
    private readonly TextModerationService _sut = new();

    [Theory]
    [InlineData("Este comentario tiene mierda")]
    [InlineData("isso e uma merda")]
    [InlineData("this is shit")]
    [InlineData("hijo de puta")]
    [InlineData("filho da puta")]
    [InlineData("go to hell")]
    [InlineData("vai tomar no cu")]
    [InlineData("vete a la mierda")]
    [InlineData("f0dase")]
    [InlineData("M0THERFUCKER")]
    public void ContainsBlockedContent_WithBadWords_ShouldReturnTrue(string input)
    {
        var result = _sut.ContainsBlockedContent(input);

        Assert.True(result);
    }

    [Fact]
    public void ContainsBlockedContent_WithCleanText_ShouldReturnFalse()
    {
        var result = _sut.ContainsBlockedContent("Forum sobre rotinas e desenvolvimento emocional");

        Assert.False(result);
    }

    [Fact]
    public void ContainsBlockedContent_WithCleanMultilingualText_ShouldReturnFalse()
    {
        var result = _sut.ContainsBlockedContent("Consejos para mejorar la rutina familiar y el vínculo emocional");

        Assert.False(result);
    }

    [Fact]
    public void ContainsBlockedContent_WithAccentsAndUpperCase_ShouldReturnTrue()
    {
        var result = _sut.ContainsBlockedContent("Isso é CARALHÓ");

        Assert.True(result);
    }
}