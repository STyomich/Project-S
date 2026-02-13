namespace UsersService.Contracts.Interfaces;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
    string RoutingKey { get; }
}
