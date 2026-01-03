using UsersService.API.Extensions;
using UsersService.Application.Extensions;
using UsersService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

// Setting providers.
var logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddProvider(new UsersService.API.Logging.FileLoggerProvider(logsDirectory));

var app = builder.Build();

app.UseCors("AllowAll");

app.UseCustomMiddlewares();

app.MapControllers();

await app.RunAsync();
