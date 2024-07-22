using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntRadioInterface;
using SmallEarthTech.AntUsbStick;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WpfUsbStickApp.ViewModels
{
    internal class MainWindowViewModel
    {
        public DeviceCapabilities DeviceCapabilities { get; }
        public string ProductDescription { get; }
        public uint SerialNumber { get; }
        public string HostVersion { get; }

        private readonly IHost _host;

        public AntDeviceCollection AntDevices { get; }
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
                Build();

            ILoggerFactory loggerFactory = _host.Services.GetRequiredService<ILoggerFactory>();

            AntRadio radio = new(loggerFactory);
            DeviceCapabilities = radio.GetDeviceCapabilities().Result;
            ProductDescription = radio.ProductDescription;
            SerialNumber = radio.SerialNumber;
            AntResponse rsp = radio.RequestMessageAndResponse(RequestMessageID.Version, 500);
            HostVersion = Encoding.Default.GetString(rsp.Payload).TrimEnd('\0');

            // log app info
            var antAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Where(asm => asm.Name!.StartsWith("Ant"));
            var logger = _host.Services.GetRequiredService<ILogger<App>>();
            logger.LogInformation("{App}", Assembly.GetExecutingAssembly().GetName().FullName);
            foreach (var asm in antAssemblies)
            {
                logger?.LogInformation("{AntAssembly}", asm.FullName);
            }

            // create the device collection
            IConfiguration configuration = _host.Services.GetRequiredService<IConfiguration>();
            byte? missedMessages = configuration.GetValue<byte?>("MissedMessages");
            int? timeout = configuration.GetValue<int?>("Timeout");
            if (timeout != null)
            {
                // app is configured to use the ANT device timeout
                AntDevices = new(radio, loggerFactory, antDeviceTimeout: (ushort)timeout);
            }
            else
            {
                // prefer configuring app to use missed message count and default to 8 messages
                AntDevices = new(radio, loggerFactory, missedMessages: missedMessages ?? 8);
            }
        }
    }
}
