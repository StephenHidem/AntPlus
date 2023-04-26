using SmallEarthTech.AntPlus;
using SmallEarthTech.AntRadioInterface;
using SmallEarthTech.AntUsbStick;
using System.IO;
using System.Text;

namespace AntPlusUsbClient.ViewModels
{
    internal class MainWindowViewModel
    {
        public AntRadio AntRadio { get; }
        public string ProductDescription { get; }
        public string SerialString { get; }
        public string HostVersion { get; }
        public AntDeviceCollection AntDevices { get; }
        public Stream AntImage => AntDevice.AntImage;

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

            // create the device collection
            AntDevices = new AntDeviceCollection(AntRadio);
        }
    }
}
