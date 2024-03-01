using AntGrpcService.Services;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;
using SmallEarthTech.AntRadioInterface;
using SmallEarthTech.AntUsbStick;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService()
        ? AppContext.BaseDirectory
        : default
});

LoggerProviderOptions.RegisterProviderOptions<
    EventLogSettings, EventLogLoggerProvider>(builder.Services);

// Set WindowsServiceLifetime
builder.Host.UseWindowsService();

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddSingleton<IAntRadio, AntRadio>();
builder.Services.AddHostedService<DiscoveryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<AntRadioService>();
app.MapGrpcService<AntChannelService>();
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
