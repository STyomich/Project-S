using UsersService.Domain.Events;
using UsersService.Domain.ValueObjects;

namespace UsersService.Domain.Entities;

public class User
{
    private readonly List<object> _domainEvents = [];
    public IReadOnlyCollection<object> DomainEvents => _domainEvents;

    public Guid Id { get; private set; }
    public Email? Email { get; private set; }
    public string? UserName { get; private set; }
    public string? PasswordHash { get; private set; }
    public bool IsActive { get; private set; }

    private User() { } // required for ORM

    public User(Email email, string userName, string passwordHash)
    {
        Id = Guid.NewGuid();
        Email = email;
        UserName = userName;
        PasswordHash = passwordHash;
        IsActive = false;

        AddDomainEvent(new UserCreatedEvent(Id, Email.Value, UserName));
    }

    public void ChangeUserName(string newUserName)
    {
        var oldUserName = UserName;
        UserName = newUserName;

        AddDomainEvent(new UserChangedUserNameEvent(Id, oldUserName!, newUserName, Email!.Value));
    }

    public void ChangeEmail(Email newEmail)
    {
        var oldEmail = Email;
        Email = newEmail;

        AddDomainEvent(new UserChangedEmailEvent(Id, UserName!, oldEmail!.Value, newEmail.Value));
    }

    public void ChangeActivation(bool activation)
    {
        IsActive = activation;

        AddDomainEvent(new UserActivationChangedEvent(Id, UserName!, Email!.Value, IsActive));
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;

        AddDomainEvent(new UserChangedPasswordEvent(Id, UserName!, Email!.Value));
    }

    private void AddDomainEvent(object @event)
        => _domainEvents.Add(@event);

    public void ClearDomainEvents()
        => _domainEvents.Clear();
}
