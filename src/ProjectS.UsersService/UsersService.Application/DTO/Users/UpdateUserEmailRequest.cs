namespace UsersService.Application.DTO.Users;

public record UpdateUserEmailRequest(
    Guid userId,
    string newEmail
);
