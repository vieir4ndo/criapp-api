using CRIApp.Application.Jobs;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace CRIApp.Application.Tests.Jobs;

public class SendReEvaluationNotificationJobTests
{
    private readonly Mock<IChildBookRepository> _childBookRepoMock = new();
    private readonly Mock<INotificationRepository> _notificationRepoMock = new();
    private readonly SendReEvaluationNotificationJob _sut;
    private static readonly DateTime Today = DateTime.UtcNow.Date;

    public SendReEvaluationNotificationJobTests()
    {
        _notificationRepoMock
            .Setup(r => r.ExistsForToday(It.IsAny<Guid>(), It.IsAny<NotificationType>(), It.IsAny<DateTime>()))
            .ReturnsAsync(false);

        _sut = new SendReEvaluationNotificationJob(
            NullLogger<SendReEvaluationNotificationJob>.Instance,
            _childBookRepoMock.Object,
            _notificationRepoMock.Object);
    }

    [Fact]
    public void ShouldNotify_SevenDaysNoActivity_ReturnsTrue()
    {
        var childBook = NotificationJobTestHelpers.BuildChildBook(NotificationJobTestHelpers.BuildCheckpoint(Today.AddDays(-7)));
        Assert.True(SendReEvaluationNotificationJob.ShouldNotify(childBook, Today));
    }

    [Fact]
    public void ShouldNotify_MoreThanSevenDays_ReturnsTrue()
    {
        var childBook = NotificationJobTestHelpers.BuildChildBook(NotificationJobTestHelpers.BuildCheckpoint(Today.AddDays(-10)));
        Assert.True(SendReEvaluationNotificationJob.ShouldNotify(childBook, Today));
    }

    [Fact]
    public void ShouldNotify_NoActivity_ReturnsTrue()
    {
        var childBook = NotificationJobTestHelpers.BuildChildBook();
        Assert.True(SendReEvaluationNotificationJob.ShouldNotify(childBook, Today));
    }

    [Fact]
    public void ShouldNotify_RecentAnswerPreventsReEvaluation()
    {
        var checkpoint = NotificationJobTestHelpers.BuildCheckpoint(Today.AddDays(-10));
        var answer = NotificationJobTestHelpers.BuildAnswer(Today.AddDays(-2));
        var childBook = NotificationJobTestHelpers.BuildChildBook(checkpoint, answer);
        Assert.False(SendReEvaluationNotificationJob.ShouldNotify(childBook, Today));
    }

    [Fact]
    public void ShouldNotify_SixDaysNoActivity_ReturnsFalse()
    {
        var childBook = NotificationJobTestHelpers.BuildChildBook(NotificationJobTestHelpers.BuildCheckpoint(Today.AddDays(-6)));
        Assert.False(SendReEvaluationNotificationJob.ShouldNotify(childBook, Today));
    }

    [Fact]
    public async Task Run_SevenDaysNoActivity_CreatesReEvaluationNotification()
    {
        var childBook = NotificationJobTestHelpers.BuildChildBook(NotificationJobTestHelpers.BuildCheckpoint(Today.AddDays(-7)));
        _childBookRepoMock.Setup(r => r.GetAllForNotifications()).ReturnsAsync([childBook]);

        await _sut.Run();

        _notificationRepoMock.Verify(r => r.Create(
            It.Is<Notification>(n => n.Type == NotificationType.ReEvaluation)), Times.Once);
    }

    [Fact]
    public async Task Run_AlreadyNotifiedToday_DoesNotDuplicate()
    {
        var childBook = NotificationJobTestHelpers.BuildChildBook(NotificationJobTestHelpers.BuildCheckpoint(Today.AddDays(-7)));
        _childBookRepoMock.Setup(r => r.GetAllForNotifications()).ReturnsAsync([childBook]);
        _notificationRepoMock
            .Setup(r => r.ExistsForToday(childBook.Id, NotificationType.ReEvaluation, Today))
            .ReturnsAsync(true);

        await _sut.Run();

        _notificationRepoMock.Verify(r => r.Create(It.IsAny<Notification>()), Times.Never);
    }
}
