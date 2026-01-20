namespace NotificationsService.Application.Interfaces;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, string routingKey);
}
