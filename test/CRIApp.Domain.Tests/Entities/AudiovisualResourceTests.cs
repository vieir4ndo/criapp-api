using CRIApp.Domain.Entities;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Domain.Tests.Entities;

public class AudiovisualResourceTests
{
    private static readonly Guid ValidAdminId = Guid.NewGuid();

    private static AudiovisualResource CreateValidResource()
        => new AudiovisualResource("Video 1", "A helpful video", "<iframe src='...'/>", ValidAdminId);

    [Fact]
    public void Create_ValidResource_ShouldSucceed()
    {
        var resource = CreateValidResource();

        Assert.NotNull(resource);
        Assert.Equal("Video 1", resource.Name);
        Assert.Equal("A helpful video", resource.Description);
        Assert.Equal("<iframe src='...'/>", resource.Link);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new AudiovisualResource("", "description", "<iframe/>", ValidAdminId));
    }

    [Fact]
    public void Create_WithNullName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new AudiovisualResource(null!, "description", "<iframe/>", ValidAdminId));
    }

    [Fact]
    public void Create_WithEmptyDescription_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new AudiovisualResource("name", "", "<iframe/>", ValidAdminId));
    }

    [Fact]
    public void Create_WithNullDescription_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new AudiovisualResource("name", null!, "<iframe/>", ValidAdminId));
    }

    [Fact]
    public void Create_WithEmptyIframe_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new AudiovisualResource("name", "description", "", ValidAdminId));
    }

    [Fact]
    public void Create_WithNullIframe_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new AudiovisualResource("name", "description", null!, ValidAdminId));
    }

}
