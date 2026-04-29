using UsersService.Domain.Events;
using UsersService.Domain.Primitives;

namespace UsersService.Domain.Entities;

public sealed class BlobInfo : AggregateRoot
{
    public Guid Id { get; private set; }
    public string Url { get; private set; }
    public string FileName { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private BlobInfo(string url, string fileName)
    {
        Id = Guid.NewGuid();
        Url = url;
        FileName = fileName;
        CreatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new BlobInfoCreatedEvent(Id, Url, FileName));
    }
}
