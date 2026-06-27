using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Domain.Tests.Entities;

public class NotificationTests
{
    private static readonly Guid ChildBookId = Guid.NewGuid();

    [Fact]
    public void Create_ShouldSetProperties()
    {
        var notification = new Notification(ChildBookId, NotificationType.Motivational);

        Assert.Equal(ChildBookId, notification.ChildBookId);
        Assert.Equal(NotificationType.Motivational, notification.Type);
        Assert.Equal(DateTime.UtcNow.Date, notification.Date);
        Assert.False(notification.Delivered);
        Assert.False(notification.Read);
    }

    [Fact]
    public void Create_WithEmptyChildBookId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => new Notification(Guid.Empty, NotificationType.Motivational));
    }

    [Theory]
    [InlineData(NotificationType.Motivational)]
    [InlineData(NotificationType.PointsRisk)]
    [InlineData(NotificationType.ReEvaluation)]
    [InlineData(NotificationType.NewAward)]
    [InlineData(NotificationType.NewRecommendations)]
    public void Create_AllTypes_ShouldSetType(NotificationType type)
    {
        var notification = new Notification(ChildBookId, type);

        Assert.Equal(type, notification.Type);
    }

    [Fact]
    public void MarkAsDelivered_ShouldSetDeliveredToTrue()
    {
        var notification = new Notification(ChildBookId, NotificationType.NewAward);

        notification.MarkAsDelivered();

        Assert.True(notification.Delivered);
        Assert.False(notification.Read);
    }

    [Fact]
    public void MarkAsRead_ShouldSetReadToTrue()
    {
        var notification = new Notification(ChildBookId, NotificationType.NewRecommendations);

        notification.MarkAsRead();

        Assert.True(notification.Read);
        Assert.False(notification.Delivered);
    }

    [Fact]
    public void MarkAsDeliveredAndRead_ShouldSetBothToTrue()
    {
        var notification = new Notification(ChildBookId, NotificationType.PointsRisk);

        notification.MarkAsDelivered();
        notification.MarkAsRead();

        Assert.True(notification.Delivered);
        Assert.True(notification.Read);
    }
}
