using NotificationsService.Domain.Enums;
using NotificationsService.Domain.Events;

namespace NotificationsService.Domain.Entities;

public sealed class Notification
{
    private readonly List<object> _domainEvents = [];
    public IReadOnlyCollection<object> DomainEvents => _domainEvents;

    public Guid Id { get; private set; }
    public string? Email { get; private set; }
    public string? Message { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public NotificationStatus Status { get; private set; }

    private Notification() { } // required for ORM

    public Notification(string email, string message, NotificationStatus status)
    {
        Id = Guid.NewGuid();
        Email = email;
        Message = message;
        CreatedAt = DateTime.UtcNow;
        Status = status;

        AddDomainEvent(new NotificationCreatedEvent(Id, Email, Status));
    }

    private void AddDomainEvent(object @event)
        => _domainEvents.Add(@event);

    public void ClearDomainEvents()
        => _domainEvents.Clear();
}
