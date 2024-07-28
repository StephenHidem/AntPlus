using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.Extensions.Hosting;
using SmallEarthTech.AntRadioInterface;
using SmallEarthTech.AntUsbStick;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WpfUsbStickApp.ViewModels
{
    internal class MainWindowViewModel
    {
        public DeviceCapabilities DeviceCapabilities { get; }
        public string ProductDescription { get; }
        public uint SerialNumber { get; }
        public string HostVersion { get; }

        private readonly IHost _host;

        public AntCollection AntDevices { get; }
        public static Stream AntImage => AntDevice.AntImage;

        public MainWindowViewModel()
        {
            // Initialize early, without access to configuration or services
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Debug(outputTemplate:
                    "[{Timestamp:HH:mm:ss.fff} {Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}") // + file or centralized logging
                .MinimumLevel.Debug()
                .CreateLogger();

            // dependency services
            _host = Host.CreateDefaultBuilder(Environment.GetCommandLineArgs()).
                UseSerilog().
                UseAntPlus().
                ConfigureServices(services =>
                {
                    // add the implementation of IAntRadio
                    services.AddSingleton<IAntRadio, AntRadio>();
                }).
                Build();

            ILoggerFactory loggerFactory = _host.Services.GetRequiredService<ILoggerFactory>();

            IAntRadio radio = _host.Services.GetRequiredService<IAntRadio>();
            DeviceCapabilities = radio.GetDeviceCapabilities().Result;
            ProductDescription = radio.ProductDescription;
            SerialNumber = radio.SerialNumber;
            HostVersion = radio.Version;

            // log app info
            var antAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Where(asm => asm.Name!.StartsWith("Ant"));
            var logger = _host.Services.GetRequiredService<ILogger<App>>();
            logger.LogInformation("{App}", Assembly.GetExecutingAssembly().GetName().FullName);
            foreach (var asm in antAssemblies)
            {
                logger?.LogInformation("{AntAssembly}", asm.FullName);
            }

            // create the device collection
            AntDevices = _host.Services.GetRequiredService<AntCollection>();
        }
    }
}
