using UsersService.Domain.Events;
using UsersService.Domain.Primitives;
using UsersService.Domain.ValueObjects;

namespace UsersService.Domain.Entities;

public class User : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid? AvatarId { get; private set; }
    public Email? Email { get; private set; }
    public string? UserName { get; private set; }
    public string? PasswordHash { get; private set; }
    public bool IsActive { get; private set; }

    private User() { } // required for ORM

    public User(Guid avatarId, Email email, string userName, string passwordHash)
    {
        Id = Guid.NewGuid();
        AvatarId = avatarId;
        Email = email;
        UserName = userName;
        PasswordHash = passwordHash;
        IsActive = false;

        RaiseDomainEvent(new UserCreatedEvent(Id, Email.Value, UserName));
    }

    public void ChangeUserName(string newUserName)
    {
        var oldUserName = UserName;
        UserName = newUserName;

        RaiseDomainEvent(new UserChangedUserNameEvent(Id, oldUserName!, newUserName, Email!.Value));
    }

    public void ChangeEmail(Email newEmail)
    {
        var oldEmail = Email;
        Email = newEmail;

        RaiseDomainEvent(new UserChangedEmailEvent(Id, UserName!, oldEmail!.Value, newEmail.Value));
    }

    public void ChangeActivation(bool activation)
    {
        IsActive = activation;

        RaiseDomainEvent(new UserActivationChangedEvent(Id, UserName!, Email!.Value, IsActive));
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;

        RaiseDomainEvent(new UserChangedPasswordEvent(Id, UserName!, Email!.Value));
    }
}
