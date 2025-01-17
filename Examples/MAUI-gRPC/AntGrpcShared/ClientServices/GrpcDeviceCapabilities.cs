using AntRadioGrpcService;
using SmallEarthTech.AntRadioInterface;

namespace AntGrpcShared.ClientServices
{
    /// <summary>
    /// Represents the device capabilities of the ANT radio.
    /// </summary>
    public class GrpcDeviceCapabilities : DeviceCapabilities
    {
        /// <summary>Initializes a new instance of the <see cref="GrpcDeviceCapabilities" /> class.</summary>
        /// <param name="capabilities">The ANT radio capabilities.</param>
        public GrpcDeviceCapabilities(GetDeviceCapabilitiesReply capabilities)
        {
            MaxANTChannels = (byte)capabilities.MaxAntChannels;
            MaxNetworks = (byte)capabilities.MaxNetworks;
            NoReceiveChannels = capabilities.NoReceiveChannels;
            NoTransmitChannels = capabilities.NoTransmitChannels;
            NoReceiveMessages = capabilities.NoReceiveMessages;
            NoTransmitMessages = capabilities.NoTransmitMessages;
            NoAckMessages = capabilities.NoAckMessages;
            NoBurstMessages = capabilities.NoBurstMessages;
            PrivateNetworks = capabilities.PrivateNetworks;
            SerialNumber = capabilities.SerialNumber;
            PerChannelTransmitPower = capabilities.PerChannelTransmitPower;
            LowPrioritySearch = capabilities.LowPrioritySearch;
            ScriptSupport = capabilities.ScriptSupport;
            SearchList = capabilities.SearchList;
            OnboardLED = capabilities.OnboardLed;
            ExtendedMessaging = capabilities.ExtendedMessaging;
            ScanModeSupport = capabilities.ScanModeSupport;
            ExtendedChannelAssignment = capabilities.ExtendedChannelAssignment;
            ProximitySearch = capabilities.ProximitySearch;
            ANTFS_Support = capabilities.AntfsSupport;
            FITSupport = capabilities.FitSupport;
            AdvancedBurst = capabilities.AdvancedBurst;
            EventBuffering = capabilities.EventBuffering;
            EventFiltering = capabilities.EventFiltering;
            HighDutySearch = capabilities.HighDutySearch;
            SearchSharing = capabilities.SearchSharing;
            SelectiveDataUpdate = capabilities.SelectiveDataUpdate;
            SingleChannelEncryption = capabilities.SingleChannelEncryption;
            MaxDataChannels = (byte)capabilities.MaxDataChannels;
        }
    }
}
