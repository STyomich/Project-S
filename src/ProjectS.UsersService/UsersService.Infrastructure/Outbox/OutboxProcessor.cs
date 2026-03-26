using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using UsersService.Application.Interfaces;
using UsersService.Infrastructure.DbContext;

namespace UsersService.Infrastructure.Outbox;

public class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;
    private const int MaxRetries = 3;
    private const int DelaySeconds = 5;

    public OutboxProcessor(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<UsersServiceDbContext>();
                var bus = scope.ServiceProvider.GetRequiredService<IEventBus>();

                var messages = await db.OutboxMessages
                    .Where(x => x.ProcessedOnUtc == null && (x.AttemptCount ?? 0) < MaxRetries)
                    .OrderBy(x => x.OccurredOnUtc)
                    .Take(20)
                    .ToListAsync(stoppingToken);

                foreach (var message in messages)
                {
                    try
                    {
                        await bus.PublishAsync(
                            message.Type,
                            message.Content,
                            message.RoutingKey,
                            stoppingToken);

                        message.ProcessedOnUtc = DateTime.UtcNow;

                        _logger.LogInformation($"Message {message.Id} published successfully");
                    }
                    catch (Exception ex)
                    {
                        message.AttemptCount = (message.AttemptCount ?? 0) + 1;

                        if (message.AttemptCount >= MaxRetries)
                        {
                            message.Error = ex.Message;
                            _logger.LogError($"Message {message.Id} failed after {MaxRetries} attempts: {ex.Message}");
                        }
                        else
                        {
                            _logger.LogWarning($"Message {message.Id} failed (attempt {message.AttemptCount}): {ex.Message}");
                        }
                    }
                }

                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"OutboxProcessor error: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(DelaySeconds), stoppingToken);
        }
    }
}