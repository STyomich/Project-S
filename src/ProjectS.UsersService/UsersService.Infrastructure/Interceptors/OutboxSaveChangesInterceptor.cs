using System.Text.Json;
using Microsoft.EntityFrameworkCore.Diagnostics;
using UsersService.Domain.Primitives;
using UsersService.Infrastructure.Outbox;

namespace UsersService.Infrastructure.Interceptors;

public class OutboxSaveChangesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var outboxMessages = context.ChangeTracker
            .Entries<AggregateRoot>()
            .SelectMany(entry =>
            {
                var events = entry.Entity.DomainEvents;
                entry.Entity.ClearDomainEvents();
                return events;
            })
            .Select(domainEvent => new OutboxMessage
            {
                OccurredOnUtc = domainEvent.OccurredOnUtc,
                Type = domainEvent.GetType().FullName!,
                Content = JsonSerializer.Serialize(
                    domainEvent,
                    domainEvent.GetType()
                )
            })
            .ToList();

        if (outboxMessages.Count > 0)
        {
            context.Set<OutboxMessage>().AddRange(outboxMessages);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
