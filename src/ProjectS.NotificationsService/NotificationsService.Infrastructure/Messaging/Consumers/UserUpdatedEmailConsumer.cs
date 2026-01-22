using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotificationsService.Application.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UsersService.Contracts.Events;

namespace NotificationsService.Infrastructure.Messaging.Consumers
{
    public sealed class UserUpdatedEmailConsumer : IEventConsumer<UserUpdatedEmailEvent>
    {
        public string RoutingKey => "user.email.updated";
        public string QueueName => "notification.user.email.updated.queue";
        private readonly RabbitMqConnection _connection;
        private readonly string _exchange;
        private readonly ILogger<UserUpdatedEmailConsumer> _logger;
        private IChannel? _channel;

        public UserUpdatedEmailConsumer(
            RabbitMqConnection connection,
            IConfiguration config,
            ILogger<UserUpdatedEmailConsumer> logger)
        {
            _connection = connection;
            _exchange = config["RABBITMQ_USERS_EXCHANGE"] ?? "user.exchange";
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            string routingKey = RoutingKey;
            string queueName = QueueName;

            _channel = await _connection.GetChannelAsync();

            //Create exchange
            await _channel.ExchangeDeclareAsync(exchange: _exchange, type: ExchangeType.Topic, durable: true);

            //Create message queue
            await _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null); //x-message-ttl | x-max-length | x-expired 

            //Bind the message to exchange
            await _channel.QueueBindAsync(queue: queueName, exchange: _exchange, routingKey: routingKey);

            AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, args) =>
            {
                byte[] body = args.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);

                if (message != null)
                {
                    var userEmailUpdatedMessage = JsonSerializer.Deserialize<UserUpdatedEmailEvent>(message);

                    _logger.LogInformation($"User email updated: {userEmailUpdatedMessage!.UserId}, Old email: {userEmailUpdatedMessage.OldEmail}, New email: {userEmailUpdatedMessage.NewEmail}");
                }
            };

            await _channel.BasicConsumeAsync(queue: queueName, consumer: consumer, autoAck: true);
        }

        public async ValueTask StopAsync()
        {
            await _connection.DisposeAsync();
        }
    }
}