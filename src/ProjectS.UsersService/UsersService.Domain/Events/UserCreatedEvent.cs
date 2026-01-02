namespace UsersService.Domain.Events;

public sealed record UserCreatedEvent(Guid userId, string email, string userName);
