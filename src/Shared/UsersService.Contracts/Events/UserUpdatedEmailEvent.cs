using UsersService.Contracts.Interfaces;

namespace UsersService.Contracts.Events;

public record UserUpdatedEmailEvent(
    Guid UserId,
    string UserName,
    string OldEmail,
    string NewEmail
) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
    public string RoutingKey { get; } = "user.email.updated";
}
