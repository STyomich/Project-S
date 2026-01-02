namespace UsersService.Domain.Events;

public sealed record UserActivationChangedEvent(Guid userId, string userName, string email, bool isActive);

