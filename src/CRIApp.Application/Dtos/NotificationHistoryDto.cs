namespace CRIApp.Application.Dtos;

public class NotificationHistoryDto
{
    public IList<NotificationDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
