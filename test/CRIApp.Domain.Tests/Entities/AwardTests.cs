using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Domain.Tests.Entities;

public class AwardTests
{
    [Fact]
    public void Create_ShouldSetTypeAndText()
    {
        var award = new Award(AwardType.HabitBuilder, "Habit Builder");

        Assert.Equal(AwardType.HabitBuilder, award.Type);
        Assert.Equal("Habit Builder", award.Text);
        Assert.Null(award.TextEn);
        Assert.Null(award.TextPt);
        Assert.Null(award.Description);
    }

    [Fact]
    public void Create_WithEmptyText_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => new Award(AwardType.HabitBuilder, ""));
    }

    [Fact]
    public void Create_WithNullText_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => new Award(AwardType.HabitBuilder, null!));
    }

    [Theory]
    [InlineData(AwardType.HabitBuilder)]
    [InlineData(AwardType.ConsistentParent)]
    [InlineData(AwardType.SilverCognitiveDevelopment)]
    [InlineData(AwardType.SilverEmotionalBonding)]
    [InlineData(AwardType.SilverDailyRoutines)]
    [InlineData(AwardType.SilverSocialInteraction)]
    [InlineData(AwardType.GoldParent)]
    public void Create_AllTypes_ShouldSetType(AwardType type)
    {
        var award = new Award(type, "text");

        Assert.Equal(type, award.Type);
    }

    [Fact]
    public void WithTranslations_ShouldSetAllFields()
    {
        var award = new Award(AwardType.GoldParent, "Gold Parent");

        var result = award.WithTranslations("Gold Parent EN", "Gold Parent PT", "Excellent parent", "Excellent parent EN", "Pai excelente PT");

        Assert.Same(award, result);
        Assert.Equal("Gold Parent EN", award.TextEn);
        Assert.Equal("Gold Parent PT", award.TextPt);
        Assert.Equal("Excellent parent", award.Description);
        Assert.Equal("Excellent parent EN", award.DescriptionEn);
        Assert.Equal("Pai excelente PT", award.DescriptionPt);
    }

    [Fact]
    public void WithTranslations_NullValues_ShouldSetNulls()
    {
        var award = new Award(AwardType.HabitBuilder, "Habit Builder");

        award.WithTranslations(null, null);

        Assert.Null(award.TextEn);
        Assert.Null(award.TextPt);
        Assert.Null(award.Description);
    }

    [Fact]
    public void WithTranslations_PartialDescription_ShouldSetOnlyProvided()
    {
        var award = new Award(AwardType.ConsistentParent, "Consistent Parent");

        award.WithTranslations("Consistent Parent EN", null, "Good parent", null, null);

        Assert.Equal("Consistent Parent EN", award.TextEn);
        Assert.Null(award.TextPt);
        Assert.Equal("Good parent", award.Description);
        Assert.Null(award.DescriptionEn);
        Assert.Null(award.DescriptionPt);
    }
}
