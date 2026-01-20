using NotificationsService.Application.Interfaces;
using UsersService.Contracts.Events;

namespace NotificationsService.Application.Consumers;

public sealed class UserUpdatedEmailConsumer : IEventConsumer<UserUpdatedEmailEvent>
{
    public string RoutingKey => "user.email.changed";
    public string QueueName => "notification.user-email-changed";
    private readonly RabbitMqConnection _connection;
    private readonly string _exchange;

    public RabbitMQEventBus(
        RabbitMqConnection connection,
        IConfiguration config)
    {
        _connection = connection;
        _exchange = config["RABBITMQ_EXCHANGE"] ?? "user";
    }

    public async Task ConsumeAsync(
        UserUpdatedEmailEvent @event,
        EventContext context,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(
            $"[{context.MessageId}] " +
            $"{@event.UserName} changed email");
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

