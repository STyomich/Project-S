using NotificationsService.Domain.Entities;

namespace NotificationsService.Domain.Repositories;

public interface INotificationsRepository
{
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);

    Task DeleteAsync(Notification notification, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
