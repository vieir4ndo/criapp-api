using CRIApp.Application.Jobs;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace CRIApp.Application.Tests.Jobs;

public class SendPointsRiskNotificationJobTests
{
    private readonly Mock<IChildBookRepository> _childBookRepoMock = new();
    private readonly Mock<INotificationRepository> _notificationRepoMock = new();
    private readonly SendPointsRiskNotificationJob _sut;
    private static readonly DateTime Today = DateTime.UtcNow.Date;

    public SendPointsRiskNotificationJobTests()
    {
        _notificationRepoMock
            .Setup(r => r.ExistsForToday(It.IsAny<Guid>(), It.IsAny<NotificationType>(), It.IsAny<DateTime>()))
            .ReturnsAsync(false);

        _sut = new SendPointsRiskNotificationJob(
            NullLogger<SendPointsRiskNotificationJob>.Instance,
            _childBookRepoMock.Object,
            _notificationRepoMock.Object);
    }

    [Fact]
    public void ShouldNotify_ExactlyThreeDays_ReturnsTrue()
    {
        var childBook = NotificationJobTestHelpers.BuildChildBook(NotificationJobTestHelpers.BuildCheckpoint(Today.AddDays(-3)));
        Assert.True(SendPointsRiskNotificationJob.ShouldNotify(childBook, Today));
    }

    [Fact]
    public void ShouldNotify_TwoDaysWithoutCheckpoint_ReturnsFalse()
    {
        var childBook = NotificationJobTestHelpers.BuildChildBook(NotificationJobTestHelpers.BuildCheckpoint(Today.AddDays(-2)));
        Assert.False(SendPointsRiskNotificationJob.ShouldNotify(childBook, Today));
    }

    [Fact]
    public void ShouldNotify_NoCheckpoints_ReturnsFalse()
    {
        var childBook = NotificationJobTestHelpers.BuildChildBook();
        Assert.False(SendPointsRiskNotificationJob.ShouldNotify(childBook, Today));
    }

    [Fact]
    public async Task Run_ThreeDaysNoCheckpoint_CreatesPointsRiskNotification()
    {
        var childBook = NotificationJobTestHelpers.BuildChildBook(NotificationJobTestHelpers.BuildCheckpoint(Today.AddDays(-3)));
        _childBookRepoMock.Setup(r => r.GetAllForNotifications()).ReturnsAsync([childBook]);

        await _sut.Run();

        _notificationRepoMock.Verify(r => r.Create(
            It.Is<Notification>(n => n.Type == NotificationType.PointsRisk)), Times.Once);
    }

    [Fact]
    public async Task Run_AlreadyNotifiedToday_DoesNotDuplicate()
    {
        var childBook = NotificationJobTestHelpers.BuildChildBook(NotificationJobTestHelpers.BuildCheckpoint(Today.AddDays(-3)));
        _childBookRepoMock.Setup(r => r.GetAllForNotifications()).ReturnsAsync([childBook]);
        _notificationRepoMock
            .Setup(r => r.ExistsForToday(childBook.Id, NotificationType.PointsRisk, Today))
            .ReturnsAsync(true);

        await _sut.Run();

        _notificationRepoMock.Verify(r => r.Create(It.IsAny<Notification>()), Times.Never);
    }
}