namespace UtilitiesService.Domain.Interfaces;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
    string RoutingKey { get; }
}
