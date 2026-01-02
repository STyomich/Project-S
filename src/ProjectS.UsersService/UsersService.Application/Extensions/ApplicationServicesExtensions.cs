using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UsersService.Application.Interfaces;
using UsersService.Application.Validation.Users;

namespace UsersService.Application.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();

        services.AddScoped<IUsersService, Services.UsersService>();

        return services;
    }
}
