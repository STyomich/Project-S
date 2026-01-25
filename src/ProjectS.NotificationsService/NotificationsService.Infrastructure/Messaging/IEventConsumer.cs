using NotificationsService.Infrastructure.Messaging;

namespace NotificationsService.Application.Interfaces;

public interface IEventConsumer<in TEvent>
{
    /// <summary>
    /// Routing key this consumer listens to (topic exchange).
    /// </summary>
    string RoutingKey { get; }

    /// <summary>
    /// Queue name (must be unique per microservice).
    /// </summary>
    string QueueName { get; }

    /// <summary>
    /// Handle incoming event.
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispose resources.
    /// </summary>
    ValueTask StopAsync();
}
