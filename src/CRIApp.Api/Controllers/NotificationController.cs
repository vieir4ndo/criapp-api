using CRIApp.Api.Controllers.Responses;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CRIApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("notifications")]
public class NotificationController : CriAppControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(
        INotificationService notificationService,
        ILogger<NotificationController> logger) : base(logger)
    {
        _notificationService = notificationService;
    }

    [HttpGet("pending")]
    public async Task<ApiResponse<NotificationDto>> GetPending()
    {
        var notifications = await _notificationService.GetPendingByUserId(GetUserId());
        return new ApiResponse<NotificationDto>(true, notifications);
    }

    [HttpGet("history")]
    public async Task<ApiResponse<NotificationHistoryDto>> GetHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var result = await _notificationService.GetHistoryByUserId(GetUserId(), page, pageSize);
        return new ApiResponse<NotificationHistoryDto>(true, result);
    }

    [HttpPatch("{id:guid}/delivered")]
    public async Task<ApiResponse<object>> MarkDelivered(Guid id)
    {
        await _notificationService.MarkAsDelivered(id);
        return new ApiResponse<object>(true);
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<ApiResponse<object>> MarkRead(Guid id)
    {
        await _notificationService.MarkAsRead(id);
        return new ApiResponse<object>(true);
    }
}
