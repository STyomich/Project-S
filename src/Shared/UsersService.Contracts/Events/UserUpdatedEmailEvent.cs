namespace UsersService.Contracts.Events;

public record UserUpdatedEmailEvent(
    Guid UserId,
    string UserName,
    string OldEmail,
    string NewEmail
);
