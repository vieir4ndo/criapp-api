using CRIApp.Application.Resources;
using CRIApp.Application.Services;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;

namespace CRIApp.Application.Tests.Services;

public class NotificationServiceTests
{
    private readonly Mock<INotificationRepository> _notificationRepositoryMock;
    private readonly Mock<IStringLocalizer<MessageTemplates>> _localizerMock;
    private readonly NotificationService _sut;

    public NotificationServiceTests()
    {
        _notificationRepositoryMock = new Mock<INotificationRepository>();
        _localizerMock = new Mock<IStringLocalizer<MessageTemplates>>();
        _localizerMock.Setup(l => l[It.IsAny<string>()]).Returns<string>(k => new LocalizedString(k, k));
        _sut = new NotificationService(_notificationRepositoryMock.Object, _localizerMock.Object);
    }

    private static Notification BuildNotification(Guid childBookId, NotificationType type)
        => new Notification(childBookId, type);

    private static Notification BuildNotificationWithChildBook(NotificationType type)
    {
        var prcId = Guid.NewGuid();
        var child = new Child("Alice", new DateTime(2020, 1, 1), Sex.Female, 6, prcId, false);
        var childBook = new ChildBook(child.Id);
        typeof(ChildBook).GetProperty(nameof(ChildBook.Child))!.SetValue(childBook, child);

        var notification = new Notification(childBook.Id, type);
        typeof(Notification).GetProperty(nameof(Notification.ChildBook))!.SetValue(notification, childBook);
        return notification;
    }

    [Fact]
    public async Task GetPendingByUserId_ShouldReturnMappedDtos()
    {
        var userId = Guid.NewGuid();
        var notifications = new List<Notification>
        {
            BuildNotificationWithChildBook(NotificationType.Motivational),
            BuildNotificationWithChildBook(NotificationType.NewAward)
        };
        _notificationRepositoryMock.Setup(r => r.GetPendingByUserId(userId)).ReturnsAsync(notifications);

        var result = await _sut.GetPendingByUserId(userId);

        Assert.Equal(2, result.Count);
        _notificationRepositoryMock.Verify(r => r.GetPendingByUserId(userId), Times.Once);
    }

    [Fact]
    public async Task GetHistoryByUserId_ShouldReturnPagedResult()
    {
        var userId = Guid.NewGuid();
        var notifications = new List<Notification> { BuildNotificationWithChildBook(NotificationType.PointsRisk) };
        _notificationRepositoryMock
            .Setup(r => r.GetHistoryByUserId(userId, 1, 5))
            .ReturnsAsync((notifications, 1));

        var result = await _sut.GetHistoryByUserId(userId, 1, 5);

        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(5, result.PageSize);
    }

    [Fact]
    public async Task MarkAsDelivered_ExistingNotification_ShouldMarkAndUpdate()
    {
        var notification = BuildNotification(Guid.NewGuid(), NotificationType.Motivational);
        _notificationRepositoryMock.Setup(r => r.GetById(notification.Id)).ReturnsAsync(notification);
        _notificationRepositoryMock.Setup(r => r.Update(notification)).Returns(Task.CompletedTask);

        await _sut.MarkAsDelivered(notification.Id);

        Assert.True(notification.Delivered);
        _notificationRepositoryMock.Verify(r => r.Update(notification), Times.Once);
    }

    [Fact]
    public async Task MarkAsDelivered_NotFound_ShouldThrowNotFoundException()
    {
        var id = Guid.NewGuid();
        _notificationRepositoryMock.Setup(r => r.GetById(id)).ReturnsAsync((Notification?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.MarkAsDelivered(id));
    }

    [Fact]
    public async Task MarkAsRead_ExistingNotification_ShouldMarkAndUpdate()
    {
        var notification = BuildNotification(Guid.NewGuid(), NotificationType.NewRecommendations);
        _notificationRepositoryMock.Setup(r => r.GetById(notification.Id)).ReturnsAsync(notification);
        _notificationRepositoryMock.Setup(r => r.Update(notification)).Returns(Task.CompletedTask);

        await _sut.MarkAsRead(notification.Id);

        Assert.True(notification.Read);
        _notificationRepositoryMock.Verify(r => r.Update(notification), Times.Once);
    }

    [Fact]
    public async Task MarkAsRead_NotFound_ShouldThrowNotFoundException()
    {
        var id = Guid.NewGuid();
        _notificationRepositoryMock.Setup(r => r.GetById(id)).ReturnsAsync((Notification?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.MarkAsRead(id));
    }

    [Fact]
    public async Task CreateIfNotExists_NotExists_ShouldCreate()
    {
        var childBookId = Guid.NewGuid();
        var type = NotificationType.Motivational;
        _notificationRepositoryMock
            .Setup(r => r.ExistsForToday(childBookId, type, It.IsAny<DateTime>()))
            .ReturnsAsync(false);
        _notificationRepositoryMock.Setup(r => r.Create(It.IsAny<Notification>())).Returns(Task.CompletedTask);

        await _sut.CreateIfNotExists(childBookId, type);

        _notificationRepositoryMock.Verify(r => r.Create(It.Is<Notification>(n =>
            n.ChildBookId == childBookId && n.Type == type)), Times.Once);
    }

    [Fact]
    public async Task CreateIfNotExists_AlreadyExists_ShouldNotCreate()
    {
        var childBookId = Guid.NewGuid();
        var type = NotificationType.PointsRisk;
        _notificationRepositoryMock
            .Setup(r => r.ExistsForToday(childBookId, type, It.IsAny<DateTime>()))
            .ReturnsAsync(true);

        await _sut.CreateIfNotExists(childBookId, type);

        _notificationRepositoryMock.Verify(r => r.Create(It.IsAny<Notification>()), Times.Never);
    }

    [Theory]
    [InlineData(NotificationType.Motivational)]
    [InlineData(NotificationType.PointsRisk)]
    [InlineData(NotificationType.ReEvaluation)]
    [InlineData(NotificationType.NewAward)]
    [InlineData(NotificationType.NewRecommendations)]
    public async Task GetPendingByUserId_AllNotificationTypes_ShouldMapWithoutError(NotificationType type)
    {
        var userId = Guid.NewGuid();
        var notifications = new List<Notification> { BuildNotificationWithChildBook(type) };
        _notificationRepositoryMock.Setup(r => r.GetPendingByUserId(userId)).ReturnsAsync(notifications);

        var result = await _sut.GetPendingByUserId(userId);

        Assert.Single(result);
        Assert.NotNull(result[0].Title);
        Assert.NotNull(result[0].Body);
    }
}
