using UsersService.Domain.Interfaces;

namespace UsersService.Domain.Events;

public sealed record UserActivationChangedEvent(Guid userId, string userName, string email, bool isActive) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}

