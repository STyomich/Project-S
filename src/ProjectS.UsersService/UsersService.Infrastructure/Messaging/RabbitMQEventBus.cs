using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using UsersService.Application.Interfaces;

namespace UsersService.Infrastructure.Messaging;

public sealed class RabbitMQEventBus : IEventBus
{
    private readonly RabbitMqConnection _connection;
    private readonly string _exchange;

    public RabbitMQEventBus(
        RabbitMqConnection connection,
        IConfiguration config)
    {
        _connection = connection;
        _exchange = config["RABBITMQ_EXCHANGE"] ?? "user";
    }

    public async Task PublishAsync<T>(T @event, string routingKey)
    {
        await using var channel = await _connection.GetChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: _exchange,
            type: ExchangeType.Topic,
            durable: true
        );

        var body = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(@event)
        );

        var props = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            MessageId = Guid.NewGuid().ToString()
        };

        await channel.BasicPublishAsync(
            exchange: _exchange,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: props,
            body: body
        );
    }
}
