using System.ComponentModel;
using Microsoft.Build.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UsersService.Application.Interfaces;
using UsersService.Domain.Repositories;
using UsersService.Infrastructure.Caching.Redis;
using UsersService.Infrastructure.DbContext;
using UsersService.Infrastructure.Repositories;

namespace UsersService.Infrastructure.Extensions;

public static class InfrastructureServicesExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionStringTemplate = configuration.GetConnectionString("PostgresConnection")!;
        string connectionString = connectionStringTemplate
          .Replace("$POSTGRES_HOST", Environment.GetEnvironmentVariable("POSTGRES_HOST"))
          .Replace("$POSTGRES_PASSWORD", Environment.GetEnvironmentVariable("POSTGRES_PASSWORD"))
          .Replace("$POSTGRES_DATABASE", Environment.GetEnvironmentVariable("POSTGRES_DATABASE"))
          .Replace("$POSTGRES_PORT", Environment.GetEnvironmentVariable("POSTGRES_PORT"))
          .Replace("$POSTGRES_USER", Environment.GetEnvironmentVariable("POSTGRES_USER"));

        services.AddDbContext<UsersServiceDbContext>(opt =>
            {
                opt.UseNpgsql(connectionString);
            });

        services.AddStackExchangeRedisCache(options =>
        {
            var host = configuration["REDIS_HOST"];
            var port = configuration["REDIS_PORT"];

            options.Configuration = $"{host}:{port}";
        });


        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}
