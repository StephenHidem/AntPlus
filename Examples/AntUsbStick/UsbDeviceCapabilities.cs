using ANT_Managed_Library;
using SmallEarthTech.AntRadioInterface;

namespace SmallEarthTech.AntUsbStick
{
    /// <summary>
    /// This class exposes the ANT radio capabilities.
    /// </summary>
    /// <seealso cref="DeviceCapabilities" />
    public class UsbDeviceCapabilities : DeviceCapabilities
    {

        /// <summary>Initializes a new instance of the <see cref="UsbDeviceCapabilities" /> class.</summary>
        /// <param name="capabilities">The capabilities.</param>
        internal UsbDeviceCapabilities(ANT_DeviceCapabilities capabilities)
        {
            MaxANTChannels = capabilities.maxANTChannels;
            MaxNetworks = capabilities.maxNetworks;
            NoReceiveChannels = capabilities.NoReceiveChannels;
            NoTransmitChannels = capabilities.NoTransmitChannels;
            NoReceiveMessages = capabilities.NoReceiveMessages;
            NoTransmitMessages = capabilities.NoTransmitMessages;
            NoAckMessages = capabilities.NoAckMessages;
            NoBurstMessages = capabilities.NoBurstMessages;
            PrivateNetworks = capabilities.PrivateNetworks;
            SerialNumber = capabilities.SerialNumber;
            PerChannelTransmitPower = capabilities.perChannelTransmitPower;
            LowPrioritySearch = capabilities.lowPrioritySearch;
            ScriptSupport = capabilities.ScriptSupport;
            SearchList = capabilities.SearchList;
            OnboardLED = capabilities.OnboardLED;
            ExtendedMessaging = capabilities.ExtendedMessaging;
            ScanModeSupport = capabilities.ScanModeSupport;
            ExtendedChannelAssignment = capabilities.ExtendedChannelAssignment;
            ProximitySearch = capabilities.ProximitySearch;
            ANTFS_Support = capabilities.FS;
            FITSupport = capabilities.FIT;
            AdvancedBurst = capabilities.AdvancedBurst;
            EventBuffering = capabilities.EventBuffering;
            EventFiltering = capabilities.EventFiltering;
            HighDutySearch = capabilities.HighDutySearch;
            SearchSharing = capabilities.SearchSharing;
            SelectiveDataUpdate = capabilities.SelectiveDataUpdate;
            SingleChannelEncryption = capabilities.SingleChannelEncryption;
            MaxDataChannels = capabilities.maxDataChannels;
        }
    }
}

