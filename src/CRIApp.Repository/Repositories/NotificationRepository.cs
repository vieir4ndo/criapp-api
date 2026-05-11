using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CRIApp.Repository.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppContext _context;

    public NotificationRepository(AppContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsForToday(Guid childBookId, NotificationType type, DateTime date)
    {
        return await _context.Notification
            .AnyAsync(x => x.ChildBookId == childBookId && x.Type == type && x.Date == date);
    }

    public async Task Create(Notification notification)
    {
        await _context.Notification.AddAsync(notification);
        await _context.SaveChangesAsync();
    }

    public async Task<IList<Notification>> GetPendingByChildBookIds(IList<Guid> childBookIds)
    {
        return await _context.Notification
            .Where(x => childBookIds.Contains(x.ChildBookId) && !x.Delivered)
            .OrderByDescending(x => x.Date)
            .ToListAsync();
    }

    public async Task<IList<Notification>> GetPendingByUserId(Guid userId)
    {
        return await _context.Notification
            .Include(n => n.ChildBook)
                .ThenInclude(cb => cb!.Child)
            .Where(n => n.ChildBook!.Child.PrcId == userId && !n.Delivered)
            .OrderByDescending(n => n.Date)
            .ToListAsync();
    }

    public async Task<IList<Notification>> GetHistoryByUserId(Guid userId, int page, int pageSize)
    {
        return await _context.Notification
            .Include(n => n.ChildBook)
                .ThenInclude(cb => cb!.Child)
            .Where(n => n.ChildBook!.Child.PrcId == userId && n.Delivered)
            .OrderByDescending(n => n.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Notification?> GetById(Guid id)
    {
        return await _context.Notification.FindAsync(id);
    }

    public async Task Update(Notification notification)
    {
        _context.Notification.Update(notification);
        await _context.SaveChangesAsync();
    }
}
