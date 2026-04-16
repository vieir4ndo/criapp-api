using CRIApp.Domain.Entities;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Domain.Tests.Entities;

public class CommentTests
{
    private static readonly Guid ValidForumId = Guid.NewGuid();
    private static readonly Guid ValidPrcId = Guid.NewGuid();

    private static Comment CreateValidComment()
        => new Comment("A comment text", ValidForumId, ValidPrcId);

    [Fact]
    public void Create_ValidComment_ShouldSucceed()
    {
        var comment = CreateValidComment();

        Assert.NotNull(comment);
        Assert.Equal("A comment text", comment.Text);
        Assert.Equal(ValidForumId, comment.ForumId);
        Assert.Equal(ValidPrcId, comment.PrcId);
    }

    [Fact]
    public void Create_WithEmptyText_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => new Comment("", ValidForumId, ValidPrcId));
    }

    [Fact]
    public void Create_WithNullText_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => new Comment(null!, ValidForumId, ValidPrcId));
    }

    [Fact]
    public void Create_WithEmptyForumId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => new Comment("text", Guid.Empty, ValidPrcId));
    }

    [Fact]
    public void Create_WithEmptyPrcId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => new Comment("text", ValidForumId, Guid.Empty));
    }
}
