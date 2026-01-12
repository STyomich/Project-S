using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace UsersService.Infrastructure.Messaging;

public sealed class RabbitMqConnection : IAsyncDisposable
{
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;

    public RabbitMqConnection(IConfiguration configuration)
    {
        _factory = new ConnectionFactory
        {
            HostName = configuration["RABBITMQ_HOST"] ?? "localhost",
            Port = int.Parse(configuration["RABBITMQ_PORT"] ?? "5672"),
            UserName = configuration["RABBITMQ_USERNAME"] ?? "user",
            Password = configuration["RABBITMQ_PASSWORD"] ?? "password",
            AutomaticRecoveryEnabled = true
        };
    }

    private async Task<IConnection> GetConnectionAsync()
    {
        if (_connection is not null && _connection.IsOpen)
            return _connection;

        _connection = await _factory.CreateConnectionAsync();
        return _connection;
    }

    public async ValueTask<IChannel> GetChannelAsync()
    {
        var connection = await GetConnectionAsync();
        return await connection.CreateChannelAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
            await _connection.CloseAsync();
    }
}
