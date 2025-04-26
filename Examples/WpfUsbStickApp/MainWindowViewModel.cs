using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntPlus.Extensions.Hosting;
using SmallEarthTech.AntRadioInterface;
using SmallEarthTech.AntUsbStick;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WpfUsbStickApp
{
    internal class MainWindowViewModel
    {
        private readonly IHost _host;

        public DeviceCapabilities DeviceCapabilities { get; }
        public string ProductDescription { get; }
        public uint SerialNumber { get; }
        public string HostVersion { get; }
        public AntCollection AntDevices { get; }
        public static Stream? AntImage => typeof(AntDevice).Assembly.GetManifestResourceStream("SmallEarthTech.AntPlus.Images.AntPlus.png");

        public MainWindowViewModel()
        {
            // Initialize Serilog early, without access to configuration or services
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Debug(outputTemplate:
                    "[{Timestamp:HH:mm:ss.fff} {Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}") // + file or centralized logging
                .CreateLogger();

            // dependency services
            _host = Host.CreateDefaultBuilder(Environment.GetCommandLineArgs()).
                UseSerilog((context, loggerConfiguration) =>
                {
                    loggerConfiguration.WriteTo.Debug(outputTemplate:
                        "[{Timestamp:HH:mm:ss.fff} {Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}");
                    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
                }).
                UseAntPlus().   // this adds all the required dependencies to use the ANT+ class library
                ConfigureServices(services =>
                {
                    // add the implementation of IAntRadio to the host
                    services.AddSingleton<IAntRadio, AntRadio>();

                    // add bike radar to the host
                    services.AddKeyedTransient<AntDevice, BikeRadar>(BikeRadar.DeviceClass);
                }).
                Build();

            IAntRadio radio = _host.Services.GetRequiredService<IAntRadio>();
            DeviceCapabilities = radio.GetDeviceCapabilities().Result;
            ProductDescription = radio.ProductDescription;
            SerialNumber = radio.SerialNumber;
            HostVersion = radio.Version;

            // log app info
            var antAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Where(asm => asm.Name!.Contains("Ant"));
            var logger = _host.Services.GetRequiredService<ILogger<App>>();
            logger.LogInformation("{App}", Assembly.GetExecutingAssembly().GetName().FullName);
            foreach (var asm in antAssemblies)
            {
                logger?.LogInformation("{AntAssembly}", asm.FullName);
            }

            // create the device collection and start scanning for broadcasting ANT devices
            AntDevices = _host.Services.GetRequiredService<AntCollection>();
            _ = AntDevices.StartScanning();
            radio.RadioResponse += (sender, e) =>
            {
                // handle radio response
                if (e.ResponseId == MessageId.StartupMessage)
                {
                    _ = AntDevices.StartScanning();
                }
            };
        }
    }
}
