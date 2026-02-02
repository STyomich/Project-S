using UsersService.Domain.Interfaces;

namespace UsersService.Domain.Events;

public sealed record UserChangedPasswordEvent(Guid userId, string userName, string email) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
