using CRIApp.Domain.Entities;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Domain.Tests.Entities;

public class ForumTests
{
    private static readonly Guid ValidPrcId = Guid.NewGuid();

    private static Forum CreateValidForum(string name = "My Forum", string description = "A description")
        => new Forum(name, description, ValidPrcId);

    [Fact]
    public void Create_ValidForum_ShouldSucceed()
    {
        var forum = CreateValidForum();

        Assert.NotNull(forum);
        Assert.Equal("My Forum", forum.Name);
        Assert.Equal("A description", forum.Description);
        Assert.Equal(ValidPrcId, forum.PrcId);
        Assert.Empty(forum.Comments);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => new Forum("", "description", ValidPrcId));
    }

    [Fact]
    public void Create_WithNullName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => new Forum(null!, "description", ValidPrcId));
    }

    [Fact]
    public void Create_WithEmptyDescription_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => new Forum("name", "", ValidPrcId));
    }

    [Fact]
    public void Create_WithNullDescription_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => new Forum("name", null!, ValidPrcId));
    }

    [Fact]
    public void Create_WithEmptyGuidPrcId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => new Forum("name", "description", Guid.Empty));
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateProperties()
    {
        var forum = CreateValidForum();

        forum.Update("New Name", "New Description");

        Assert.Equal("New Name", forum.Name);
        Assert.Equal("New Description", forum.Description);
    }

    [Fact]
    public void Update_WithNullValues_ShouldKeepExistingValues()
    {
        var forum = CreateValidForum();
        var originalName = forum.Name;
        var originalDescription = forum.Description;

        forum.Update(null, null);

        Assert.Equal(originalName, forum.Name);
        Assert.Equal(originalDescription, forum.Description);
    }
}
