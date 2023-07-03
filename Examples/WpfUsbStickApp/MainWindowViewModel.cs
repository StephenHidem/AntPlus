using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntRadioInterface;
using SmallEarthTech.AntUsbStick;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WpfUsbStickApp.ViewModels
{
    internal class MainWindowViewModel
    {
        public AntRadio AntRadio { get; }
        public string ProductDescription { get; }
        public string SerialString { get; }
        public string HostVersion { get; }

        private readonly IHost _host;

        public AntDeviceCollection AntDevices { get; }
        public static Stream AntImage => AntDevice.AntImage;

        public MainWindowViewModel()
        {
            AntRadio = new AntRadio();
            ProductDescription = AntRadio.GetProductDescription();
            SerialString = AntRadio.GetSerialString();
            AntResponse rsp = AntRadio.RequestMessageAndResponse(RequestMessageID.Version, 500);
            HostVersion = Encoding.Default.GetString(rsp.Payload).TrimEnd('\0');

            AntRadio.SetNetworkKey(0, new byte[] { 0xB9, 0xA5, 0x21, 0xFB, 0xBD, 0x72, 0xC3, 0x45 });
            AntRadio.EnableRxExtendedMessages(true);
            AntRadio.GetChannel(0).AssignChannel(ChannelType.BaseSlaveReceive, 0, 500);
            AntRadio.GetChannel(0).SetChannelID(new ChannelId(0), 500);
            AntRadio.GetChannel(0).SetChannelFreq(57, 500);
            AntRadio.OpenRxScanMode();

            AntRadio.GetChannel(1).AssignChannel(ChannelType.BaseSlaveReceive, 0, 500);

            // dependency services
            _host = Host.CreateDefaultBuilder().Build();

            // log app info
            var antAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Where(asm => asm.Name.StartsWith("Ant"));
            var logger = _host.Services.GetRequiredService<ILogger<App>>();
            logger.LogInformation("{App}", Assembly.GetExecutingAssembly().GetName().FullName);
            foreach (var asm in antAssemblies)
            {
                logger?.LogInformation("{AntAssembly}", asm.FullName);
            }

            // create the device collection
            AntDevices = new AntDeviceCollection(AntRadio, _host.Services.GetService<ILoggerFactory>(), 2000);
        }
    }
}
