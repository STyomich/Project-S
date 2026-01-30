using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UtilitiesService.Domain.Repositories;
using UtilitiesService.Infrastructure.Persistence;
using UtilitiesService.Infrastructure.Repositories;

namespace UtilitiesService.Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            return new UtilitiesDbContext(config);
        });

        services.AddScoped<IUtilitiesRepository, UtilitiesRepository>();

        return services;
    }
}
