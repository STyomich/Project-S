namespace UsersService.Application.DTO.Users;

public record RegisterUserRequest(
    string UserName,
    string Email,
    string Password
);
