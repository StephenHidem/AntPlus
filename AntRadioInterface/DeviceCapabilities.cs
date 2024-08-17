namespace SmallEarthTech.AntRadioInterface
{
    /// <summary>The radio capabilities class.</summary>
    public abstract class DeviceCapabilities
    {
        /// <summary>
        /// Number of channels available.
        /// </summary>
        public byte MaxANTChannels { get; protected set; }   //byte 0

        /// <summary>
        /// Number of simultaneous networks allowed
        /// </summary>
        public byte MaxNetworks { get; protected set; }      //byte 1

        /// <summary>Gets a value indicating whether [no receive channels].</summary>
        /// <value>
        ///   <c>true</c> if [no receive channels]; otherwise, <c>false</c>.</value>
        public bool NoReceiveChannels { get; protected set; }
        /// <summary>Gets a value indicating whether [no transmit channels].</summary>
        /// <value>
        ///   <c>true</c> if [no transmit channels]; otherwise, <c>false</c>.</value>
        public bool NoTransmitChannels { get; protected set; }
        /// <summary>Gets a value indicating whether [no receive messages].</summary>
        /// <value>
        ///   <c>true</c> if [no receive messages]; otherwise, <c>false</c>.</value>
        public bool NoReceiveMessages { get; protected set; }
        /// <summary>Gets a value indicating whether [no transmit messages].</summary>
        /// <value>
        ///   <c>true</c> if [no transmit messages]; otherwise, <c>false</c>.</value>
        public bool NoTransmitMessages { get; protected set; }
        /// <summary>Gets a value indicating whether [no ack messages].</summary>
        /// <value>
        ///   <c>true</c> if [no ack messages]; otherwise, <c>false</c>.</value>
        public bool NoAckMessages { get; protected set; }
        /// <summary>Gets a value indicating whether [no burst messages].</summary>
        /// <value>
        ///   <c>true</c> if [no burst messages]; otherwise, <c>false</c>.</value>
        public bool NoBurstMessages { get; protected set; }

        /// <summary>Gets a value indicating whether private networks are supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        public bool PrivateNetworks { get; protected set; }
        /// <summary>Gets a value indicating whether serial number is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        public bool SerialNumber { get; protected set; }
        /// <summary>Gets a value indicating whether per channel transmit power is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        public bool PerChannelTransmitPower { get; protected set; }
        /// <summary>Gets a value indicating whether low priority search is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        public bool LowPrioritySearch { get; protected set; }
        /// <summary>Gets a value indicating whether script is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        public bool ScriptSupport { get; protected set; }
        /// <summary>Gets a value indicating whether search list is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        public bool SearchList { get; protected set; }
        /// <summary>Gets a value indicating whether onboard led is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        public bool OnboardLED { get; protected set; }
        /// <summary>Gets a value indicating whether extended messaging is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        public bool ExtendedMessaging { get; protected set; }
        /// <summary>Gets a value indicating whether scan mode supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        public bool ScanModeSupport { get; protected set; }
        /// <summary>Gets a value indicating whether extended channel assignment is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        public bool ExtendedChannelAssignment { get; protected set; }
        /// <summary>Gets a value indicating whether proximity search is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        public bool ProximitySearch { get; protected set; }
        /// <summary>Gets a value indicating whether this <see cref="DeviceCapabilities" /> is ANTFS supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        public bool ANTFS_Support { get; protected set; }
        /// <summary>Gets a value indicating whether this <see cref="DeviceCapabilities" /> is FIT file compatible.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        public bool FITSupport { get; protected set; }

        /// <summary>Gets a value indicating whether advanced burst is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        public bool AdvancedBurst { get; protected set; }
        /// <summary>Gets a value indicating whether event buffering is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        public bool EventBuffering { get; protected set; }
        /// <summary>Gets a value indicating whether event filtering is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        public bool EventFiltering { get; protected set; }
        /// <summary>Gets a value indicating whether high duty search is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        public bool HighDutySearch { get; protected set; }
        /// <summary>Gets a value indicating whether search sharing is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        public bool SearchSharing { get; protected set; }
        /// <summary>Gets a value indicating whether selective data update is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        public bool SelectiveDataUpdate { get; protected set; }
        /// <summary>Gets a value indicating whether single channel encryption is supported.</summary>
        /// <value>
        ///   <c>true</c> if supported; otherwise, <c>false</c>.</value>
        public bool SingleChannelEncryption { get; protected set; }

        /// <summary>
        /// Number of SensRcore data channels available
        /// </summary>
        public byte MaxDataChannels { get; protected set; }  //byte 5
    }
}
