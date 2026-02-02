namespace UsersService.Domain.Interfaces;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
