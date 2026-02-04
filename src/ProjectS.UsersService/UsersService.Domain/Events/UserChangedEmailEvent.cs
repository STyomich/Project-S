using UsersService.Domain.Interfaces;

namespace UsersService.Domain.Events;

public record UserChangedEmailEvent(
    Guid UserId,
    string UserName,
    string OldEmail,
    string NewEmail
) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
    public string RoutingKey { get; } = "user.email.changed";
}
