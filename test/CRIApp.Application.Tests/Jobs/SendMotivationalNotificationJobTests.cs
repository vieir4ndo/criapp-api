using CRIApp.Application.Abstractions;
using CRIApp.Application.Jobs;
using CRIApp.Domain.Data;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace CRIApp.Application.Tests.Jobs;

public class SendMotivationalNotificationJobTests
{
    private readonly Mock<IChildBookRepository> _childBookRepoMock = new();
    private readonly Mock<INotificationService> _notificationServiceMock = new();
    private readonly SendMotivationalNotificationJob _sut;
    private static readonly DateTime Today = DateTime.UtcNow.Date;

    public SendMotivationalNotificationJobTests()
    {
        _sut = new SendMotivationalNotificationJob(
            NullLogger<SendMotivationalNotificationJob>.Instance,
            _childBookRepoMock.Object,
            _notificationServiceMock.Object);
    }

    [Fact]
    public void ShouldNotify_ExactlyTwoDays_ReturnsTrue()
    {
        var childBook = NotificationJobTestHelpers.BuildChildBook(NotificationJobTestHelpers.BuildCheckpoint(Today.AddDays(-2)));
        Assert.True(SendMotivationalNotificationJob.ShouldNotify(childBook, Today));
    }

    [Fact]
    public void ShouldNotify_OneDayWithoutCheckpoint_ReturnsFalse()
    {
        var childBook = NotificationJobTestHelpers.BuildChildBook(NotificationJobTestHelpers.BuildCheckpoint(Today.AddDays(-1)));
        Assert.False(SendMotivationalNotificationJob.ShouldNotify(childBook, Today));
    }

    [Fact]
    public void ShouldNotify_ThreeDaysWithoutCheckpoint_ReturnsFalse()
    {
        var childBook = NotificationJobTestHelpers.BuildChildBook(NotificationJobTestHelpers.BuildCheckpoint(Today.AddDays(-3)));
        Assert.False(SendMotivationalNotificationJob.ShouldNotify(childBook, Today));
    }

    [Fact]
    public void ShouldNotify_NoCheckpoints_ReturnsFalse()
    {
        var childBook = NotificationJobTestHelpers.BuildChildBook();
        Assert.False(SendMotivationalNotificationJob.ShouldNotify(childBook, Today));
    }

    [Fact]
    public async Task Run_TwoDaysNoCheckpoint_CreatesMotivationalNotification()
    {
        var childBook = NotificationJobTestHelpers.BuildChildBook(NotificationJobTestHelpers.BuildCheckpoint(Today.AddDays(-2)));
        _childBookRepoMock.Setup(r => r.GetAllForNotifications()).ReturnsAsync([childBook]);

        await _sut.Run();

        _notificationServiceMock.Verify(
            r => r.CreateIfNotExists(It.IsAny<Guid>(), NotificationType.Motivational), Times.Once);
    }
}
