using Microsoft.Extensions.Hosting;
using NotificationsService.Application.Interfaces;
using UsersService.Contracts.Events;

namespace NotificationsService.Infrastructure.Messaging.Consumers.HostedServices;

public sealed class UserUpdatedEmailHostedService(
    IEventConsumer<UserUpdatedEmailEvent> consumer) : IHostedService
{
    private readonly IEventConsumer<UserUpdatedEmailEvent> _consumer = consumer;
    public Task StartAsync(CancellationToken cancellationToken)
        => _consumer.StartAsync(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken)
        => _consumer.StopAsync().AsTask();
}
