using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationsService.Application.Interfaces;
using UsersService.Contracts.Events;

namespace NotificationsService.Infrastructure.Messaging.Consumers.HostedServices;

public sealed class UserUpdatedEmailHostedService(
    IServiceScopeFactory serviceScopeFactory) : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private IEventConsumer<UserUpdatedEmailEvent>? _consumer;
    private IServiceScope? _scope;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _scope = _serviceScopeFactory.CreateScope();
        _consumer = _scope.ServiceProvider.GetRequiredService<IEventConsumer<UserUpdatedEmailEvent>>();
        await _consumer.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_consumer is not null)
        {
            await _consumer.StopAsync();
        }

        _scope?.Dispose();
    }
}