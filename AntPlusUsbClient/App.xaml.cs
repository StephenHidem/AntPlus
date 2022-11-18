using AntPlus;
using AntRadioInterface;
using AntUsbStick;
using System.Windows;

namespace AntPlusUsbClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IAntRadio AntRadio { get; set; }
        public static AntDeviceCollection AntDevices { get; set; }

        public App()
        {
            AntRadio = new AntRadio();
            AntRadio.SetNetworkKey(0, new byte[] { 0xB9, 0xA5, 0x21, 0xFB, 0xBD, 0x72, 0xC3, 0x45 });
            AntRadio.EnableRxExtendedMessages(true);
            //App.AntRadio.SetLibConfig(AntRadioInterface.LibConfigFlags.MesgOutIncRssi | AntRadioInterface.LibConfigFlags.MesgOutIncTimeStamp | AntRadioInterface.LibConfigFlags.MesgOutIncDeviceId);
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
