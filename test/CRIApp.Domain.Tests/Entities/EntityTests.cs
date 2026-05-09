using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using Xunit;

namespace CRIApp.Domain.Tests.Entities;

public class EntityTests
{
    private static User BuildUser(string username = "user@domain.com")
        => new User("Test", new DateTime(1990, 1, 1), Sex.Male, username, "pass123");

    [Fact]
    public void Equals_SameInstance_ShouldReturnTrue()
    {
        var user = BuildUser();

        Assert.True(user.Equals(user));
    }

    [Fact]
    public void Equals_DifferentInstanceSameId_ShouldReturnTrue()
    {
        var user = BuildUser();

        Assert.True(user.Equals(user));
    }

    [Fact]
    public void Equals_DifferentEntities_ShouldReturnFalse()
    {
        var user1 = BuildUser("a@domain.com");
        var user2 = BuildUser("b@domain.com");

        Assert.False(user1.Equals(user2));
    }

    [Fact]
    public void Equals_NullComparison_ShouldReturnFalse()
    {
        var user = BuildUser();

        Assert.False(user.Equals(null));
    }

    [Fact]
    public void Equals_NonEntityObject_ShouldReturnFalse()
    {
        var user = BuildUser();

        Assert.False(user.Equals("not an entity"));
    }

    [Fact]
    public void EqualityOperator_TwoNulls_ShouldReturnTrue()
    {
        User? a = null;
        User? b = null;

        Assert.True(a == b);
    }

    [Fact]
    public void EqualityOperator_LeftNull_ShouldReturnFalse()
    {
        User? a = null;
        var b = BuildUser();

        Assert.False(a == b);
    }

    [Fact]
    public void EqualityOperator_RightNull_ShouldReturnFalse()
    {
        var a = BuildUser();
        User? b = null;

        Assert.False(a == b);
    }

    [Fact]
    public void EqualityOperator_SameEntity_ShouldReturnTrue()
    {
        var user = BuildUser();

        Assert.True(user == user);
    }

    [Fact]
    public void InequalityOperator_DifferentEntities_ShouldReturnTrue()
    {
        var user1 = BuildUser("a@domain.com");
        var user2 = BuildUser("b@domain.com");

        Assert.True(user1 != user2);
    }

    [Fact]
    public void GetHashCode_SameEntity_ShouldReturnSameValue()
    {
        var user = BuildUser();

        Assert.Equal(user.GetHashCode(), user.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentEntities_ShouldReturnDifferentValues()
    {
        var user1 = BuildUser("a@domain.com");
        var user2 = BuildUser("b@domain.com");

        Assert.NotEqual(user1.GetHashCode(), user2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldContainClassNameAndId()
    {
        var user = BuildUser();

        var result = user.ToString();

        Assert.Contains("User", result);
        Assert.Contains(user.Id.ToString(), result);
    }
}
