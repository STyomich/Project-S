using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationsService.Application.Interfaces;
using NotificationsService.Domain.Repositories;
using NotificationsService.Infrastructure.Messaging;
using NotificationsService.Infrastructure.Messaging.Consumers;
using NotificationsService.Infrastructure.Messaging.Consumers.HostedServices;
using NotificationsService.Infrastructure.Persistence;
using NotificationsService.Infrastructure.Repositories;
using UsersService.Contracts.Events;

namespace NotificationsService.Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionStringTemplate = configuration.GetConnectionString("MySqlConnection")!;
        string connectionString = connectionStringTemplate
          .Replace("$MYSQL_HOST", Environment.GetEnvironmentVariable("MYSQL_HOST"))
          .Replace("$MYSQL_PASSWORD", Environment.GetEnvironmentVariable("MYSQL_PASSWORD"))
          .Replace("$MYSQL_DATABASE", Environment.GetEnvironmentVariable("MYSQL_DATABASE"))
          .Replace("$MYSQL_PORT", Environment.GetEnvironmentVariable("MYSQL_PORT"))
          .Replace("$MYSQL_USER", Environment.GetEnvironmentVariable("MYSQL_USER"));
        services.AddDbContext<NotificationsDbContext>(options =>
        {
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)));
        });

        services.AddSingleton<RabbitMqConnection>();
        services.AddSingleton<IEventBus, RabbitMQEventBus>();

        services.AddScoped<INotificationsRepository, NotificationsRepository>();
        services.AddScoped<IEventConsumer<UserUpdatedEmailEvent>, UserUpdatedEmailConsumer>();

        services.AddHostedService<UserUpdatedEmailHostedService>();

        return services;
    }
}
