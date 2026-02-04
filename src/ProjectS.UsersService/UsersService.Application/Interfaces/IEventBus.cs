namespace UsersService.Application.Interfaces;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, string routingKey, CancellationToken cancellationToken = default);

    // Used by OutboxProcessor (raw)
    Task PublishAsync(string eventType, string payload, string routingKey, CancellationToken cancellationToken = default);
}
