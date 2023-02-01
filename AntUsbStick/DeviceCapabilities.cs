using ANT_Managed_Library;
using SmallEarthTech.AntRadioInterface;

namespace SmallEarthTech.AntUsbStick
{
    /// <summary>
    /// This class exposes the ANT radio capabilities.
    /// </summary>
    /// <seealso cref="SmallEarthTech.AntRadioInterface.IDeviceCapabilities" />
    public class DeviceCapabilities : IDeviceCapabilities
    {
        private readonly ANT_DeviceCapabilities capabilities;

        /// <inheritdoc/>
        public byte MaxANTChannels => capabilities.maxANTChannels;

        /// <inheritdoc/>
        public byte MaxNetworks => capabilities.maxNetworks;

        /// <inheritdoc/>
        public bool NoReceiveChannels => capabilities.NoReceiveChannels;

        /// <inheritdoc/>
        public bool NoTransmitChannels => capabilities.NoTransmitChannels;

        /// <inheritdoc/>
        public bool NoReceiveMessages => capabilities.NoReceiveMessages;

        /// <inheritdoc/>
        public bool NoTransmitMessages => capabilities.NoTransmitMessages;

        /// <inheritdoc/>
        public bool NoAckMessages => capabilities.NoAckMessages;

        /// <inheritdoc/>
        public bool NoBurstMessages => capabilities.NoBurstMessages;

        /// <inheritdoc/>
        public bool PrivateNetworks => capabilities.PrivateNetworks;

        /// <inheritdoc/>
        public bool SerialNumber => capabilities.SerialNumber;

        /// <inheritdoc/>
        public bool PerChannelTransmitPower => capabilities.perChannelTransmitPower;

        /// <inheritdoc/>
        public bool LowPrioritySearch => capabilities.lowPrioritySearch;

        /// <inheritdoc/>
        public bool ScriptSupport => capabilities.ScriptSupport;

        /// <inheritdoc/>
        public bool SearchList => capabilities.SearchList;

        /// <inheritdoc/>
        public bool OnboardLED => capabilities.OnboardLED;

        /// <inheritdoc/>
        public bool ExtendedMessaging => capabilities.ExtendedMessaging;

        /// <inheritdoc/>
        public bool ScanModeSupport => capabilities.ScanModeSupport;

        /// <inheritdoc/>
        public bool ExtendedChannelAssignment => capabilities.ExtendedChannelAssignment;

        /// <inheritdoc/>
        public bool ProximitySearch => capabilities.ProximitySearch;

        /// <inheritdoc/>
        public bool ANTFS_Support => capabilities.FS;

        /// <inheritdoc/>
        public bool FITSupport => capabilities.FIT;

        /// <inheritdoc/>
        public bool AdvancedBurst => capabilities.AdvancedBurst;

        /// <inheritdoc/>
        public bool EventBuffering => capabilities.EventBuffering;

        /// <inheritdoc/>
        public bool EventFiltering => capabilities.EventFiltering;

        /// <inheritdoc/>
        public bool HighDutySearch => capabilities.HighDutySearch;

        /// <inheritdoc/>
        public bool SearchSharing => capabilities.SearchSharing;

        /// <inheritdoc/>
        public bool SelectiveDataUpdate => capabilities.SelectiveDataUpdate;

        /// <inheritdoc/>
        public bool SingleChannelEncryption => capabilities.SingleChannelEncryption;

        /// <inheritdoc/>
        public byte MaxDataChannels => capabilities.maxDataChannels;

        /// <summary>Initializes a new instance of the <see cref="DeviceCapabilities" /> class.</summary>
        /// <param name="capabilities">The capabilities.</param>
        public DeviceCapabilities(ANT_DeviceCapabilities capabilities)
        {
            this.capabilities = capabilities;
        }
    }
}
