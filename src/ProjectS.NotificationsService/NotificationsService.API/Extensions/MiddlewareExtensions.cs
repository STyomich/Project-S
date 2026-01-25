using NotificationsService.API.Middlewares;

namespace NotificationsService.API.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        app.UseMiddleware<RequestResponseLoggingMiddleware>();
        return app;
    }
}
