using CRIApp.Domain.Entities;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Domain.Tests.Entities;

public class RecommendationCheckpointTests
{
    [Fact]
    public void Create_ValidCheckpoint_ShouldSetProperties()
    {
        var childBookRecommendationId = Guid.NewGuid();

        var checkpoint = new RecommendationCheckpoint(childBookRecommendationId, true);

        Assert.NotEqual(Guid.Empty, checkpoint.Id);
        Assert.Equal(childBookRecommendationId, checkpoint.ChildBookRecommendationId);
        Assert.Equal(DateTime.UtcNow.Date, checkpoint.Date);
        Assert.True(checkpoint.Completed);
    }

    [Fact]
    public void Create_WithCompletedFalse_ShouldSetCompletedToFalse()
    {
        var checkpoint = new RecommendationCheckpoint(Guid.NewGuid(), false);

        Assert.False(checkpoint.Completed);
    }

    [Fact]
    public void Create_WithEmptyChildBookRecommendationId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new RecommendationCheckpoint(Guid.Empty, true));
    }

    [Fact]
    public void Create_TwoCheckpoints_ShouldHaveDifferentIds()
    {
        var id = Guid.NewGuid();

        var c1 = new RecommendationCheckpoint(id, true);
        var c2 = new RecommendationCheckpoint(id, true);

        Assert.NotEqual(c1.Id, c2.Id);
    }
}
