using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using Xunit;

namespace CRIApp.Domain.Tests.Entities;

public class ChildBookRecommendationTests
{
    private static RecommendationCheckpoint BuildCheckpointOnDate(Guid cbrId, bool completed, DateTime date)
    {
        var cp = new RecommendationCheckpoint(cbrId, completed);
        typeof(RecommendationCheckpoint)
            .GetProperty(nameof(RecommendationCheckpoint.Date))!
            .GetSetMethod(nonPublic: true)!
            .Invoke(cp, new object[] { date });
        return cp;
    }

    [Fact]
    public void IsAssimilated_NoCheckpoints_ShouldReturnFalse()
    {
        var cbr = new ChildBookRecommendation();

        Assert.False(cbr.IsAssimilated());
    }

    [Fact]
    public void IsAssimilated_ThreeDistinctCompletedDaysInWindow_ShouldReturnTrue()
    {
        var cbr = new ChildBookRecommendation();
        var today = DateTime.UtcNow.Date;
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, true, today));
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, true, today.AddDays(-1)));
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, true, today.AddDays(-2)));

        Assert.True(cbr.IsAssimilated());
    }

    [Fact]
    public void IsAssimilated_TwoCompletedDaysInWindow_ShouldReturnFalse()
    {
        var cbr = new ChildBookRecommendation();
        var today = DateTime.UtcNow.Date;
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, true, today));
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, true, today.AddDays(-1)));

        Assert.False(cbr.IsAssimilated());
    }

    [Fact]
    public void IsAssimilated_ThreeDaysButAllCompleted_False_ShouldReturnFalse()
    {
        var cbr = new ChildBookRecommendation();
        var today = DateTime.UtcNow.Date;
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, false, today));
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, false, today.AddDays(-1)));
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, false, today.AddDays(-2)));

        Assert.False(cbr.IsAssimilated());
    }

    [Fact]
    public void IsAssimilated_ThreeCompletedDaysOutsideWindow_ShouldReturnFalse()
    {
        var cbr = new ChildBookRecommendation();
        var today = DateTime.UtcNow.Date;
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, true, today.AddDays(-7)));
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, true, today.AddDays(-8)));
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, true, today.AddDays(-9)));

        Assert.False(cbr.IsAssimilated());
    }

    [Fact]
    public void IsCompletedToday_NoCheckpoints_ShouldReturnNull()
    {
        var cbr = new ChildBookRecommendation();

        Assert.Null(cbr.IsCompletedToday());
    }

    [Fact]
    public void IsCompletedToday_CheckpointTodayCompleted_ShouldReturnTrue()
    {
        var cbr = new ChildBookRecommendation();
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, true, DateTime.UtcNow.Date));

        Assert.True(cbr.IsCompletedToday());
    }

    [Fact]
    public void IsCompletedToday_CheckpointTodayNotCompleted_ShouldReturnFalse()
    {
        var cbr = new ChildBookRecommendation();
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, false, DateTime.UtcNow.Date));

        Assert.False(cbr.IsCompletedToday());
    }

    [Fact]
    public void IsCompletedToday_CheckpointYesterdayOnly_ShouldReturnNull()
    {
        var cbr = new ChildBookRecommendation();
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, true, DateTime.UtcNow.Date.AddDays(-1)));

        Assert.Null(cbr.IsCompletedToday());
    }

    [Fact]
    public void GetCompletedCountInWindow_NoCheckpoints_ShouldReturnZero()
    {
        var cbr = new ChildBookRecommendation();

        Assert.Equal(0, cbr.GetCompletedCountInWindow());
    }

    [Fact]
    public void GetCompletedCountInWindow_TwoCompletedDays_ShouldReturnTwo()
    {
        var cbr = new ChildBookRecommendation();
        var today = DateTime.UtcNow.Date;
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, true, today));
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, true, today.AddDays(-1)));

        Assert.Equal(2, cbr.GetCompletedCountInWindow());
    }

    [Fact]
    public void GetCompletedCountInWindow_DuplicateDates_ShouldCountOnce()
    {
        var cbr = new ChildBookRecommendation();
        var today = DateTime.UtcNow.Date;
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, true, today));
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, true, today));

        Assert.Equal(1, cbr.GetCompletedCountInWindow());
    }
}
