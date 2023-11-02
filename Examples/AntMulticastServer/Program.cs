// See https://aka.ms/new-console-template for more information
using AntMulticastServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SmallEarthTech.AntRadioInterface;
using SmallEarthTech.AntUsbStick;
using System.Net.Sockets;
using System.Reflection;

// opening credits
Console.WriteLine(string.Format("ANT Multicast Server - Version {0}.", Assembly.GetExecutingAssembly().GetName().Version?.ToString(3)));
Console.WriteLine("Copyright 2023 Stephen Hidem.");

// Initialize early, without access to configuration or services
Log.Logger = new LoggerConfiguration()
    .WriteTo.Debug(outputTemplate:
        "[{Timestamp:HH:mm:ss.fff} {Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}") // + file or centralized logging
    .MinimumLevel.Debug()
    .CreateLogger();

// dependency services
IHost host = Host.CreateDefaultBuilder(args).
    UseSerilog().
    ConfigureServices(s =>
    {
        s.AddHostedService<AntMessageService>();
        s.AddSingleton<IAntRadio, AntRadio>();
        s.AddSingleton<UdpClient>(sp => new UdpClient(2000, AddressFamily.InterNetworkV6));
    }).
    Build();

// run until terminated
host.Run();
