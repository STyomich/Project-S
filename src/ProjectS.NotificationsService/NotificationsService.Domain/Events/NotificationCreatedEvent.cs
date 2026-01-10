using NotificationsService.Domain.Enums;

namespace NotificationsService.Domain.Events;

public sealed record NotificationCreatedEvent(Guid id, string email, NotificationStatus status);
