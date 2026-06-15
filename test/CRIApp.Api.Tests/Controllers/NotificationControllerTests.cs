using CRIApp.Api.Controllers;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CRIApp.Api.Tests.Controllers;

public class NotificationControllerTests
{
    private readonly Mock<INotificationService> _serviceMock;
    private readonly Mock<ILogger<NotificationController>> _loggerMock;
    private readonly NotificationController _sut;
    private readonly Guid _userId = Guid.NewGuid();

    public NotificationControllerTests()
    {
        _serviceMock = new Mock<INotificationService>();
        _loggerMock = new Mock<ILogger<NotificationController>>();
        _sut = new NotificationController(_serviceMock.Object, _loggerMock.Object)
        {
            ControllerContext = ControllerContextFactory.CreateAuthenticated(_userId)
        };
    }

    private static NotificationDto BuildDto(NotificationType type = NotificationType.Motivational)
    {
        var notification = new Notification(Guid.NewGuid(), type);
        return new NotificationDto(notification) { Title = "T", Body = "B" };
    }

    [Fact]
    public async Task GetPending_ShouldReturnNotifications()
    {
        var dtos = new List<NotificationDto> { BuildDto(), BuildDto(NotificationType.NewAward) };
        _serviceMock.Setup(s => s.GetPendingByUserId(_userId)).ReturnsAsync(dtos);

        var result = await _sut.GetPending();

        Assert.True(result.Success);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task GetHistory_ShouldReturnPagedResult()
    {
        var historyDto = new NotificationHistoryDto
        {
            Items = new List<NotificationDto> { BuildDto() },
            TotalCount = 1,
            Page = 1,
            PageSize = 5
        };
        _serviceMock.Setup(s => s.GetHistoryByUserId(_userId, 1, 5)).ReturnsAsync(historyDto);

        var result = await _sut.GetHistory(1, 5);

        Assert.True(result.Success);
        Assert.Single(result.Data.Single().Items);
    }

    [Fact]
    public async Task MarkDelivered_ExistingId_ShouldReturnSuccess()
    {
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.MarkAsDelivered(id)).Returns(Task.CompletedTask);

        var result = await _sut.MarkDelivered(id);

        Assert.True(result.Success);
        _serviceMock.Verify(s => s.MarkAsDelivered(id), Times.Once);
    }

    [Fact]
    public async Task MarkDelivered_NotFound_ShouldPropagate()
    {
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.MarkAsDelivered(id)).ThrowsAsync(new NotFoundException("not found"));

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.MarkDelivered(id));
    }

    [Fact]
    public async Task MarkRead_ExistingId_ShouldReturnSuccess()
    {
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.MarkAsRead(id)).Returns(Task.CompletedTask);

        var result = await _sut.MarkRead(id);

        Assert.True(result.Success);
        _serviceMock.Verify(s => s.MarkAsRead(id), Times.Once);
    }

    [Fact]
    public async Task MarkRead_NotFound_ShouldPropagate()
    {
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.MarkAsRead(id)).ThrowsAsync(new NotFoundException("not found"));

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.MarkRead(id));
    }
}
