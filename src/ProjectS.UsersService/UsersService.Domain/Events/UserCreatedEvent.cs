using UsersService.Domain.Interfaces;

namespace UsersService.Domain.Events;

public sealed record UserCreatedEvent(Guid userId, string email, string userName) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
