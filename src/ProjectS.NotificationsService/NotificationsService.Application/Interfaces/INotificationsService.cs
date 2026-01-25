using NotificationsService.Domain.Enums;

namespace NotificationsService.Application.Interfaces;

public interface INotificationsService
{
    /// <summary>
    /// Saves a notification for a user.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="email">Actual email of user.</param>
    /// <param name="message">Message to be sent.</param>
    /// <param name="status">Status of the notification.</param>
    /// <returns></returns>
    Task SaveNotificationAsync(Guid userId, string? email, string? message, NotificationStatus status);
}
