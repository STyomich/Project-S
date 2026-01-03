using UsersService.Application.DTO.Users;

namespace UsersService.Application.Interfaces;

public interface IUsersService
{
    /// <summary>
    /// Logs in a user and returns a JWT token
    /// </summary>
    /// <param name="email">Email of the user</param>
    /// <param name="password">Password of the user</param>
    /// <returns>JWT token as string</returns>
    Task<string> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets short info about user by userId
    /// </summary>
    /// <param name="userId">Guid of the user</param>
    /// <returns>UserShortInfoDto</returns>
    Task<UserShortInfoDto> GetUserShortInfoAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="registerUserRequest">RegisterUserRequest DTO</param>
    Task RegisterUserAsync(RegisterUserRequest registerUserRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user exists by email
    /// </summary>
    /// <param name="email">Email of the user</param>
    /// <returns>True if user exists, false otherwise</returns>
    Task<bool> IsUserExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates a user by userId
    /// </summary>
    /// <param name="userId">Guid of the user</param>
    Task DeactivateUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates user's email
    /// </summary>
    /// <param name="request">UpdateUserEmailRequest DTO</param>
    Task UpdateUserEmailAsync(UpdateUserEmailRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates user's username
    /// </summary>
    /// <param name="request">UpdateUserNameRequest DTO</param>
    Task UpdateUserNameAsync(UpdateUserNameRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates user's password
    /// </summary>
    /// <param name="request">UpdateUserPasswordRequest DTO</param>
    Task UpdateUserPasswordAsync(UpdateUserPasswordRequest request, CancellationToken cancellationToken = default);
}
