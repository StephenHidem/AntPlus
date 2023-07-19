using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        public AntRadio UsbAntRadio { get; }
        public string ProductDescription { get; }
        public string SerialString { get; }
        public string HostVersion { get; }

        private readonly IHost _host;

        public AntDeviceCollection AntDevices { get; }
        public static Stream AntImage => AntDevice.AntImage;

        public MainWindowViewModel()
        {
            // dependency services
            _host = Host.CreateDefaultBuilder(Environment.GetCommandLineArgs()).
                ConfigureServices(s =>
                {
                    s.AddSingleton<IAntRadio, AntRadio>();
                    s.AddSingleton<AntDeviceCollection>();
                }).
                Build();

            UsbAntRadio = (AntRadio)_host.Services.GetRequiredService<IAntRadio>();
            ProductDescription = UsbAntRadio.GetProductDescription();
            SerialString = UsbAntRadio.GetSerialString();
            AntResponse rsp = UsbAntRadio.RequestMessageAndResponse(RequestMessageID.Version, 500);
            HostVersion = Encoding.Default.GetString(rsp.Payload).TrimEnd('\0');

            UsbAntRadio.SetNetworkKey(0, new byte[] { 0xB9, 0xA5, 0x21, 0xFB, 0xBD, 0x72, 0xC3, 0x45 });
            UsbAntRadio.EnableRxExtendedMessages(true);
            IAntChannel antChannel = UsbAntRadio.GetChannel(0);
            antChannel.AssignChannel(ChannelType.BaseSlaveReceive, 0, 500);
            antChannel.SetChannelID(new ChannelId(0), 500);
            antChannel.SetChannelFreq(57, 500);
            UsbAntRadio.OpenRxScanMode();

            //UsbAntRadio.GetChannel(1).AssignChannel(ChannelType.BaseSlaveReceive, 0, 500);

            // log app info
            var antAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Where(asm => asm.Name.StartsWith("Ant"));
            var logger = _host.Services.GetRequiredService<ILogger<App>>();
            logger.LogInformation("{App}", Assembly.GetExecutingAssembly().GetName().FullName);
            foreach (var asm in antAssemblies)
            {
                logger?.LogInformation("{AntAssembly}", asm.FullName);
            }

            // create the device collection
            AntDevices = _host.Services.GetRequiredService<AntDeviceCollection>();
        }
    }
}
