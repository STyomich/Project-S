namespace UsersService.Domain.Events;

public record UserChangedEmailEvent(
    Guid UserId,
    string UserName,
    string OldEmail,
    string NewEmail
);
