namespace NotificationsService.Infrastructure.Messaging;

public sealed record EventContext(
    string MessageId,
    string Exchange,
    string RoutingKey,
    DateTime ReceivedAtUtc,
    IReadOnlyDictionary<string, object?> Headers
);
