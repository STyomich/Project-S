using UsersService.Domain.Exceptions;

namespace UsersService.Domain.ValueObjects;

public sealed class Email
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
            throw new ValidationException("Invalid email");

        return new Email(value.ToLower());
    }

    public override int GetHashCode() => Value.GetHashCode();
}
