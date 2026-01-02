namespace UsersService.Domain.Exceptions;

public sealed class ValidationException : DomainException
{
    public ValidationException(string message) : base("Validation exception in creating of entity.") { }
}
