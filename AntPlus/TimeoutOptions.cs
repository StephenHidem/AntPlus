namespace SmallEarthTech.AntPlus
{
    /// <summary>
    /// This class provides timeout options. The AntCollection class uses the options pattern and
    /// propagates a timeout to <see cref="AntDevice"/>s that are instantiated by AntCollection.
    /// </summary>
    /// <remarks>
    /// The application host builder retrieves configuration options from various sources such as the command line or
    /// appsettings.json. If the configuration can't find TimeoutOptions, the collection will default to <see cref="MissedMessages"/>.
    /// If both <see cref="Timeout"/> and <see cref="MissedMessages"/> are defined. Timeout is ignored and MissedMessages is used.
    /// Prefer MissedMessages as this will scale the ANT device timeout based on the specific data broadcast rate of an ANT device.
    /// Asset trackers broadcast 16 times a second and Geocaches broadcast once every 2 seconds. Other devices typically
    /// broadcast 4 times a second.
    /// <para>
    /// For example, your appsettings.json file would have this entry -
    /// <code>
    /// {
    ///   "TimeoutOptions": {
    ///     "MissedMessages": 8,
    ///     "Timeout": null
    ///   }
    /// }
    /// </code>
    /// The command line could define the timeout "--TimeoutOptions:MissedMessages=8".
    /// </para>
    /// <para>Timeouts can be disabled by setting Timeout to -1 and ignoring or setting MissedMessages to null.</para>
    /// </remarks>
    public sealed class TimeoutOptions
    {
        /// <summary>
        /// Gets or sets the timeout before an ANT device goes offline.
        /// </summary>
        /// <value>
        /// The timeout in milliseconds. Set to -1 to disable device timeout.
        /// </value>
        public int? Timeout { get; set; }
        /// <summary>
        /// Gets or sets the number of missed messages before an ANT device goes offline.
        /// </summary>
        /// <value>
        /// The number of missed messages.
        /// </value>
        public byte? MissedMessages { get; set; }
    }
}