namespace UsersService.Application.DTO.Users;

public record UpdateUserNameRequest(
    Guid userId,
    string newUserName
);
