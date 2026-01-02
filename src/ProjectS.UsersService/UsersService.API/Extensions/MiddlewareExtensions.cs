using UsersService.API.Middlewares;

namespace UsersService.API.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        app.UseMiddleware<RequestResponseLoggingMiddleware>();
        // app.UseMiddleware<TokenValidationMiddleware>();
        return app;
    }
}
