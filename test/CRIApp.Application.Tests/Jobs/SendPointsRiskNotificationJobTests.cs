using CRIApp.Application.Abstractions;
using CRIApp.Application.Jobs;
using CRIApp.Domain.Data;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace CRIApp.Application.Tests.Jobs;

public class SendPointsRiskNotificationJobTests
{
    private readonly Mock<IChildBookRepository> _childBookRepoMock = new();
    private readonly Mock<INotificationService> _notificationServiceMock = new();
    private readonly SendPointsRiskNotificationJob _sut;
    private static readonly DateTime Today = DateTime.UtcNow.Date;

    public SendPointsRiskNotificationJobTests()
    {
        _sut = new SendPointsRiskNotificationJob(
            NullLogger<SendPointsRiskNotificationJob>.Instance,
            _childBookRepoMock.Object,
            _notificationServiceMock.Object);
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

        _notificationServiceMock.Verify(
            r => r.CreateIfNotExists(It.IsAny<Guid>(), NotificationType.PointsRisk), Times.Once);
    }
}