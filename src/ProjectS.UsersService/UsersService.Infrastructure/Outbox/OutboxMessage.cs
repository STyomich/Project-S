namespace UsersService.Infrastructure.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; init; }
    public string Type { get; init; } = default!;
    public string RoutingKey { get; init; } = default!;
    public string Content { get; init; } = default!;
    public DateTime? ProcessedOnUtc { get; set; }
    public string? Error { get; set; }
}
