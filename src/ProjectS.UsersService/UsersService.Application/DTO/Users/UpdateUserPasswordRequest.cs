namespace UsersService.Application.DTO.Users;

public record UpdateUserPasswordRequest(
    Guid userId,
    string newPassword
);
