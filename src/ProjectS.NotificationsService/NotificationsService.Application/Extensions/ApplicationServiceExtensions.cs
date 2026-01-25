using Microsoft.Extensions.DependencyInjection;
using NotificationsService.Application.Interfaces;

namespace NotificationsService.Application.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationsService, Services.NotificationsService>();

        return services;
    }
}
