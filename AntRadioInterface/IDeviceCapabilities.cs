namespace SmallEarthTech.AntRadioInterface
{
    /// <summary>The radio capabilities interface.</summary>
    public interface IDeviceCapabilities
    {
        /// <summary>
        /// Number of channels available.
        /// </summary>
        byte MaxANTChannels { get; }   //byte 0

        /// <summary>
        /// Number of simultaneous networks allowed
        /// </summary>
        byte MaxNetworks { get; }      //byte 1

        /// <summary>Gets a value indicating whether [no receive channels].</summary>
        /// <value>
        ///   <c>true</c> if [no receive channels]; otherwise, <c>false</c>.</value>
        bool NoReceiveChannels { get; }
        /// <summary>Gets a value indicating whether [no transmit channels].</summary>
        /// <value>
        ///   <c>true</c> if [no transmit channels]; otherwise, <c>false</c>.</value>
        bool NoTransmitChannels { get; }
        /// <summary>Gets a value indicating whether [no receive messages].</summary>
        /// <value>
        ///   <c>true</c> if [no receive messages]; otherwise, <c>false</c>.</value>
        bool NoReceiveMessages { get; }
        /// <summary>Gets a value indicating whether [no transmit messages].</summary>
        /// <value>
        ///   <c>true</c> if [no transmit messages]; otherwise, <c>false</c>.</value>
        bool NoTransmitMessages { get; }
        /// <summary>Gets a value indicating whether [no ack messages].</summary>
        /// <value>
        ///   <c>true</c> if [no ack messages]; otherwise, <c>false</c>.</value>
        bool NoAckMessages { get; }
        /// <summary>Gets a value indicating whether [no burst messages].</summary>
        /// <value>
        ///   <c>true</c> if [no burst messages]; otherwise, <c>false</c>.</value>
        bool NoBurstMessages { get; }

        /// <summary>Gets a value indicating whether private networks are supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        bool PrivateNetworks { get; }
        /// <summary>Gets a value indicating whether serial number is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        bool SerialNumber { get; }
        /// <summary>Gets a value indicating whether per channel transmit power is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        bool PerChannelTransmitPower { get; }
        /// <summary>Gets a value indicating whether low priority search is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        bool LowPrioritySearch { get; }
        /// <summary>Gets a value indicating whether script is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        bool ScriptSupport { get; }
        /// <summary>Gets a value indicating whether search list is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        bool SearchList { get; }
        /// <summary>Gets a value indicating whether onboard led is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        bool OnboardLED { get; }
        /// <summary>Gets a value indicating whether extended messaging is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        bool ExtendedMessaging { get; }
        /// <summary>Gets a value indicating whether scan mode supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        bool ScanModeSupport { get; }
        /// <summary>Gets a value indicating whether extended channel assignment is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        bool ExtendedChannelAssignment { get; }
        /// <summary>Gets a value indicating whether proximity search is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        bool ProximitySearch { get; }
        /// <summary>Gets a value indicating whether this <see cref="IDeviceCapabilities" /> is ANTFS supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        bool ANTFS_Support { get; }
        /// <summary>Gets a value indicating whether this <see cref="IDeviceCapabilities" /> is FIT file compatible.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        bool FITSupport { get; }

        /// <summary>Gets a value indicating whether advanced burst is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        bool AdvancedBurst { get; }
        /// <summary>Gets a value indicating whether event buffering is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        bool EventBuffering { get; }
        /// <summary>Gets a value indicating whether event filtering is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        bool EventFiltering { get; }
        /// <summary>Gets a value indicating whether high duty search is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        bool HighDutySearch { get; }
        /// <summary>Gets a value indicating whether search sharing is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        bool SearchSharing { get; }
        /// <summary>Gets a value indicating whether selective data update is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        bool SelectiveDataUpdate { get; }
        /// <summary>Gets a value indicating whether single channel encryption is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        bool SingleChannelEncryption { get; }

        /// <summary>
        /// Number of SensRcore data channels available
        /// </summary>
        byte MaxDataChannels { get; }  //byte 5
    }
}
