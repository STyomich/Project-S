using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Polly;
using Microsoft.Extensions.Logging;

namespace NotificationsService.Infrastructure.Messaging;

public sealed class RabbitMqConnection : IAsyncDisposable
{
    private readonly ConnectionFactory _factory;
    private readonly ILogger<RabbitMqConnection> _logger;
    private IConnection? _connection;

    public RabbitMqConnection(
        IConfiguration configuration,
        ILogger<RabbitMqConnection> logger)
    {
        _logger = logger;

        _factory = new ConnectionFactory
        {
            HostName = configuration["RABBITMQ_HOST"] ?? "localhost",
            Port = int.Parse(configuration["RABBITMQ_PORT"] ?? "5672"),
            UserName = configuration["RABBITMQ_USERNAME"] ?? "user",
            Password = configuration["RABBITMQ_PASSWORD"] ?? "password",
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
        };
    }

    private async Task<IConnection> GetConnectionAsync(
        CancellationToken cancellationToken = default)
    {
        if (_connection is { IsOpen: true })
            return _connection;

        var policy = Policy
            .Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetryAsync(
                retryCount: 10,
                sleepDurationProvider: retry =>
                    TimeSpan.FromSeconds(3),
                onRetry: (ex, delay) =>
                {
                    _logger.LogWarning(
                        ex,
                        "RabbitMQ not ready. Retrying in {Delay}s...",
                        delay.TotalSeconds);
                });

        _connection = await policy.ExecuteAsync(
            ct => _factory.CreateConnectionAsync(ct),
            cancellationToken);

        return _connection;
    }

    public async ValueTask<IChannel> GetChannelAsync(
        CancellationToken cancellationToken = default)
    {
        var connection = await GetConnectionAsync(cancellationToken);
        return await connection.CreateChannelAsync(cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
            await _connection.CloseAsync();
    }
}
