using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UtilitiesService.Application.Interfaces;
using UtilitiesService.Application.Policies;

namespace UtilitiesService.Application.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("UsersMicroservice", client =>
        {
            client.BaseAddress = new Uri($"http://{configuration["USERS_MICROSERVICE_NAME"]}:{configuration["USERS_MICROSERVICE_PORT"]}");
        })
        .AddPolicyHandler(
            services.BuildServiceProvider().GetRequiredService<UsersMicroservicePolicies>().GetCombinedPolicy());

        services.AddScoped<IUtilitiesService, Services.UtilitiesService>();

        return services;
    }
}
