using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NotificationsService.Application.Consumers;

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
    Task ConsumeAsync(
        TEvent @event,
        EventContext context,
        CancellationToken cancellationToken = default);
}
