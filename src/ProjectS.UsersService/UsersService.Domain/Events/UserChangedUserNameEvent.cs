namespace UsersService.Domain.Events;

public sealed record UserChangedUserNameEvent(Guid userId, string oldUserName, string newUserName, string email);
