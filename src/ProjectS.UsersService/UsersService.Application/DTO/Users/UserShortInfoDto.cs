namespace UsersService.Application.DTO.Users;

public record UserShortInfoDto(
    Guid Id,
    string UserName,
    string Email
);