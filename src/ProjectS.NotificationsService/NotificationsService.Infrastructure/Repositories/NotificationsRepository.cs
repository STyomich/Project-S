using NotificationsService.Domain.Entities;
using NotificationsService.Domain.Repositories;
using NotificationsService.Infrastructure.Persistence;

namespace NotificationsService.Infrastructure.Repositories;

public sealed class NotificationsRepository(NotificationsDbContext context) : INotificationsRepository
{
    private readonly NotificationsDbContext _context = context;

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _context.Notifications.AddAsync(notification, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications.FindAsync(id, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
