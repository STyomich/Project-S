using Microsoft.Extensions.DependencyInjection;
using UtilitiesService.Application.Interfaces;

namespace UtilitiesService.Application.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUtilitiesService, Services.UtilitiesService>();

        return services;
    }
}
