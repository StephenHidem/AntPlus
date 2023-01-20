namespace SmallEarthTech.AntRadioInterface
{
    /// <summary>The radio capabilities interface.</summary>
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

        /// <summary>Gets a value indicating whether [private networks].</summary>
        /// <value>
        ///   <c>true</c> if [private networks]; otherwise, <c>false</c>.</value>
        bool PrivateNetworks { get; }
        /// <summary>Gets a value indicating whether [serial number].</summary>
        /// <value>
        ///   <c>true</c> if [serial number]; otherwise, <c>false</c>.</value>
        bool SerialNumber { get; }
        /// <summary>Gets a value indicating whether [per channel transmit power].</summary>
        /// <value>
        ///   <c>true</c> if [per channel transmit power]; otherwise, <c>false</c>.</value>
        bool PerChannelTransmitPower { get; }
        /// <summary>Gets a value indicating whether [low priority search].</summary>
        /// <value>
        ///   <c>true</c> if [low priority search]; otherwise, <c>false</c>.</value>
        bool LowPrioritySearch { get; }
        /// <summary>Gets a value indicating whether [script support].</summary>
        /// <value>
        ///   <c>true</c> if [script support]; otherwise, <c>false</c>.</value>
        bool ScriptSupport { get; }
        /// <summary>Gets a value indicating whether [search list].</summary>
        /// <value>
        ///   <c>true</c> if [search list]; otherwise, <c>false</c>.</value>
        bool SearchList { get; }

        /// <summary>Gets a value indicating whether [onboard led].</summary>
        /// <value>
        ///   <c>true</c> if [onboard led]; otherwise, <c>false</c>.</value>
        bool OnboardLED { get; }
        /// <summary>Gets a value indicating whether [extended messaging].</summary>
        /// <value>
        ///   <c>true</c> if [extended messaging]; otherwise, <c>false</c>.</value>
        bool ExtendedMessaging { get; }
        /// <summary>Gets a value indicating whether [scan mode support].</summary>
        /// <value>
        ///   <c>true</c> if [scan mode support]; otherwise, <c>false</c>.</value>
        bool ScanModeSupport { get; }
        /// <summary>Gets a value indicating whether [extended channel assignment].</summary>
        /// <value>
        ///   <c>true</c> if [extended channel assignment]; otherwise, <c>false</c>.</value>
        bool ExtendedChannelAssignment { get; }
        /// <summary>Gets a value indicating whether [proximity search].</summary>
        /// <value>
        ///   <c>true</c> if [proximity search]; otherwise, <c>false</c>.</value>
        bool ProximitySearch { get; }
        /// <summary>Gets a value indicating whether this <see cref="IDeviceCapabilities" /> is fs.</summary>
        /// <value>
        ///   <c>true</c> if fs; otherwise, <c>false</c>.</value>
        bool FS { get; }
        /// <summary>Gets a value indicating whether this <see cref="IDeviceCapabilities" /> is FIT file compatible.</summary>
        /// <value>
        ///   <c>true</c> if FIT supported; otherwise, <c>false</c>.</value>
        bool FIT { get; }

        /// <summary>Gets a value indicating whether [advanced burst].</summary>
        /// <value>
        ///   <c>true</c> if [advanced burst]; otherwise, <c>false</c>.</value>
        bool AdvancedBurst { get; }
        /// <summary>Gets a value indicating whether [event buffering].</summary>
        /// <value>
        ///   <c>true</c> if [event buffering]; otherwise, <c>false</c>.</value>
        bool EventBuffering { get; }
        /// <summary>Gets a value indicating whether [event filtering].</summary>
        /// <value>
        ///   <c>true</c> if [event filtering]; otherwise, <c>false</c>.</value>
        bool EventFiltering { get; }
        /// <summary>Gets a value indicating whether [high duty search].</summary>
        /// <value>
        ///   <c>true</c> if [high duty search]; otherwise, <c>false</c>.</value>
        bool HighDutySearch { get; }
        /// <summary>Gets a value indicating whether [search sharing].</summary>
        /// <value>
        ///   <c>true</c> if [search sharing]; otherwise, <c>false</c>.</value>
        bool SearchSharing { get; }
        /// <summary>Gets a value indicating whether [selective data update].</summary>
        /// <value>
        ///   <c>true</c> if [selective data update]; otherwise, <c>false</c>.</value>
        bool SelectiveDataUpdate { get; }
        /// <summary>Gets a value indicating whether [single channel encryption].</summary>
        /// <value>
        ///   <c>true</c> if [single channel encryption]; otherwise, <c>false</c>.</value>
        bool SingleChannelEncryption { get; }

        /// <summary>
        /// Number of SensRcore data channels available
        /// </summary>
        byte MaxDataChannels { get; }  //byte 5
    }
}
