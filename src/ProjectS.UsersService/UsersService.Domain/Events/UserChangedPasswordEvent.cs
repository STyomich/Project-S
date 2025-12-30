namespace UsersService.Domain.Events;

public sealed record UserChangedPasswordEvent(Guid userId, string userName, string email);
