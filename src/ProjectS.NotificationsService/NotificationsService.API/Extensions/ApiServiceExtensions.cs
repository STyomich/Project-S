using NotificationsService.API.Policies;

namespace NotificationsService.API.Extensions;

public static class ApiServiceExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("Allow808X", builder =>
            {
                builder
                    .SetIsOriginAllowed(origin =>
                    {
                        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                            return false;

                        return uri.Port >= 8080 && uri.Port <= 8089;
                    })
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        services.AddHttpClient("ExternalApi", client =>
        {
            client.BaseAddress = new Uri("http://localhost:XXXX/"); // replace, implemented as scaffold
            client.Timeout = Timeout.InfiniteTimeSpan; // Polly handles timeout
        })
        .AddPolicyHandler(PollyPolicies.ResiliencePolicy);


        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();

        return services;
    }
}
