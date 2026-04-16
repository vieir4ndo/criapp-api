using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Domain.Tests.Entities;

public class ChildTests
{
    private static readonly Guid ValidPrcId = Guid.NewGuid();

    private static Child CreateValidChild(string name = "John")
        => new Child(name, new DateTime(2020, 3, 15), Sex.Male, 6, ValidPrcId, false);

    [Fact]
    public void Create_ValidChild_ShouldSucceed()
    {
        var child = CreateValidChild();

        Assert.NotNull(child);
        Assert.Equal("John", child.Name);
        Assert.Equal(Sex.Male, child.Sex);
        Assert.Equal(6, child.MonthsOfBreastFeeding);
        Assert.Equal(ValidPrcId, child.PrcId);
        Assert.False(child.IsPremature);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Child("", new DateTime(2020, 3, 15), Sex.Male, 6, ValidPrcId, false));
    }

    [Fact]
    public void Create_WithNullName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Child(null!, new DateTime(2020, 3, 15), Sex.Male, 6, ValidPrcId, false));
    }

    [Fact]
    public void Create_WithMinValueBirthDate_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Child("John", DateTime.MinValue, Sex.Male, 6, ValidPrcId, false));
    }

    [Fact]
    public void Create_WithEmptyGuidPrcId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Child("John", new DateTime(2020, 3, 15), Sex.Male, 6, Guid.Empty, false));
    }

    [Fact]
    public void Create_WithNegativeMonthsOfBreastFeeding_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Child("John", new DateTime(2020, 3, 15), Sex.Male, -1, ValidPrcId, false));
    }

    [Fact]
    public void Create_WithZeroMonthsOfBreastFeeding_ShouldSucceed()
    {
        var child = new Child("John", new DateTime(2020, 3, 15), Sex.Male, 0, ValidPrcId, false);

        Assert.Equal(0, child.MonthsOfBreastFeeding);
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateProperties()
    {
        var child = CreateValidChild();
        var newBirthDate = new DateTime(2021, 6, 1);

        child.Update("Jane", newBirthDate, Sex.Female, 3, true);

        Assert.Equal("Jane", child.Name);
        Assert.Equal(newBirthDate, child.BirthDate);
        Assert.Equal(Sex.Female, child.Sex);
        Assert.Equal(3, child.MonthsOfBreastFeeding);
        Assert.True(child.IsPremature);
    }

    [Fact]
    public void Update_WithNullValues_ShouldKeepExistingValues()
    {
        var child = CreateValidChild();
        var originalName = child.Name;
        var originalBirthDate = child.BirthDate;

        child.Update(null, null, null, null, null);

        Assert.Equal(originalName, child.Name);
        Assert.Equal(originalBirthDate, child.BirthDate);
    }
}
