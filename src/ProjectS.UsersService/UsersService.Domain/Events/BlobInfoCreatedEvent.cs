using UsersService.Domain.Interfaces;

namespace UsersService.Domain.Events;

public sealed record BlobInfoCreatedEvent(Guid Id, string Url, string FileName) : IDomainEvent
{
    public DateTime OccurredOnUtc => DateTime.UtcNow;

    public string RoutingKey => throw new NotImplementedException();
}