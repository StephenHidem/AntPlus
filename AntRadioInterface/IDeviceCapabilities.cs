namespace AntRadioInterface
{
    public interface IDeviceCapabilities
    {
        /// <summary>
        /// Number of channels available
        /// </summary>
        byte MaxANTChannels { get; }   //byte 0

        /// <summary>
        /// Number of simultaneous networks allowed
        /// </summary>
        byte MaxNetworks { get; }      //byte 1

        //TODO: Document all the capabilities

        //Basic Capabilities - byte 2
        bool NoReceiveChannels { get; }
        bool NoTransmitChannels { get; }
        bool NoReceiveMessages { get; }
        bool NoTransmitMessages { get; }
        bool NoAckMessages { get; }
        bool NoBurstMessages { get; }

        //Advanced Capabilities - byte 3
        bool PrivateNetworks { get; }
        bool SerialNumber { get; }
        bool PerChannelTransmitPower { get; }
        bool LowPrioritySearch { get; }
        bool ScriptSupport { get; }
        bool SearchList { get; }

        //Advanced Capabilities 2 - byte 4
        bool OnboardLED { get; }
        bool ExtendedMessaging { get; }
        bool ScanModeSupport { get; }
        bool ExtendedChannelAssignment { get; }
        bool ProximitySearch { get; }
        bool FS { get; }
        bool FIT { get; }

        //Advanced Capabilities 3 - byte 6
        bool AdvancedBurst { get; }
        bool EventBuffering { get; }
        bool EventFiltering { get; }
        bool HighDutySearch { get; }
        bool SearchSharing { get; }
        bool SelectiveDataUpdate { get; }
        bool SingleChannelEncryption { get; }

        /// <summary>
        /// Number of SensRcore data channels available
        /// </summary>
        byte MaxDataChannels { get; }  //byte 5
    }
}
