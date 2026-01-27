using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UtilitiesService.Infrastructure.Persistence;

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

        return services;
    }
}
