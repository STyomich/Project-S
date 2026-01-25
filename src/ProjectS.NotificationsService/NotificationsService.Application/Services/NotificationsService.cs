using NotificationsService.Application.Interfaces;
using NotificationsService.Domain.Entities;
using NotificationsService.Domain.Enums;
using NotificationsService.Domain.Repositories;

namespace NotificationsService.Application.Services;

public class NotificationsService(INotificationsRepository notificationsRepository) : INotificationsService
{
    private readonly INotificationsRepository _notificationsRepository = notificationsRepository;
    public async Task SaveNotificationAsync(Guid userId, string? email, string? message, NotificationStatus status)
    {
        var notification = new Notification(email!, message!, status);

        await _notificationsRepository.AddAsync(notification);
    }
}
