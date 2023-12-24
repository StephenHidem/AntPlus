using AntGrpcServer.Services;
using SmallEarthTech.AntRadioInterface;
using SmallEarthTech.AntUsbStick;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddHostedService<DiscoveryService>();
builder.Services.AddSingleton<IAntRadio, AntRadio>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
app.MapGrpcService<AntRadioService>();
app.MapGrpcService<AntChannelService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
