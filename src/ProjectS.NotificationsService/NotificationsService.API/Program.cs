using NotificationsService.API.Extensions;
using NotificationsService.Application.Extensions;
using NotificationsService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UseCors("Allow808X");

app.UseCustomMiddlewares();

app.MapControllers();

await app.RunAsync();
