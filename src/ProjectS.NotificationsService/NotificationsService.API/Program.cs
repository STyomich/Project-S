using NotificationsService.API.Extensions;
using NotificationsService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UseCors("Allow808X");

app.MapControllers();

await app.RunAsync();
