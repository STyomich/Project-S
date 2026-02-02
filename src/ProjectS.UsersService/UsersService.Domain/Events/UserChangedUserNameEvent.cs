using UsersService.Domain.Interfaces;

namespace UsersService.Domain.Events;

public sealed record UserChangedUserNameEvent(Guid userId, string oldUserName, string newUserName, string email) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
