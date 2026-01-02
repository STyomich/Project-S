using UsersService.Application.DTO.Users;
using UsersService.Application.Interfaces;
using UsersService.Domain.Entities;
using UsersService.Domain.Repositories;
using UsersService.Domain.ValueObjects;

namespace UsersService.Application.Services;

public class UsersService(IUsersRepository usersRepository) : IUsersService
{
    private readonly IUsersRepository _usersRepository = usersRepository;

    public async Task DeactivateUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _usersRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"User with id {userId} not found.");

        user.ChangeActivation(false);

        await _usersRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserShortInfoDto> GetUserShortInfoAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _usersRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"User with id {userId} not found.");

        return new UserShortInfoDto
        (
            user.Id,
            user.UserName!,
            user.Email!.Value
        );
    }

    public async Task<bool> IsUserExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var isExists = await _usersRepository.GetByEmailAsync(email, cancellationToken);
        return isExists != null;
    }

    public async Task RegisterUserAsync(RegisterUserRequest registerUserRequest, CancellationToken cancellationToken = default)
    {
        var email = Email.Create(registerUserRequest.Email);
        var user = new User(email, registerUserRequest.UserName, registerUserRequest.Password.Hash());

        await _usersRepository.AddAsync(user, cancellationToken);
        int saveChanges = await _usersRepository.SaveChangesAsync(cancellationToken);

        if (saveChanges == 0)
            throw new Exception("Failed to save user.");
    }

    public async Task UpdateUserEmailAsync(UpdateUserEmailRequest request, CancellationToken cancellationToken = default)
    {
        var email = Email.Create(request.newEmail);

        var user = await _usersRepository.GetByIdAsync(request.userId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"User with id {request.userId} not found.");

        user.ChangeEmail(email);    

        var saveChanges = await _usersRepository.SaveChangesAsync(cancellationToken);
        if (saveChanges == 0)
            throw new Exception("Failed to update user email.");
    }

    public async Task UpdateUserNameAsync(UpdateUserNameRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _usersRepository.GetByIdAsync(request.userId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"User with id {request.userId} not found.");

        user.ChangeUserName(request.newUserName);

        var saveChanges = await _usersRepository.SaveChangesAsync(cancellationToken);
        if (saveChanges == 0)
            throw new Exception("Failed to update user name.");
    }

    public async Task UpdateUserPasswordAsync(UpdateUserPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _usersRepository.GetByIdAsync(request.userId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"User with id {request.userId} not found.");

        user.ChangePassword(request.newPassword.Hash());

        var saveChanges = await _usersRepository.SaveChangesAsync(cancellationToken);
        if (saveChanges == 0)
            throw new Exception("Failed to update user password.");
    }
}
