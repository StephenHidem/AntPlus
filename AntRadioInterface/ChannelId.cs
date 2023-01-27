namespace SmallEarthTech.AntRadioInterface
{
    /// <summary>The channel ID is a concrete class comprised of device number, device type, and transmission type.</summary>
    public class ChannelId
    {
        /// <summary>Gets the channel identifier.</summary>
        /// <value>The channel identifier.</value>
        public uint Id { get; }
        /// <summary>Gets the type of the device.</summary>
        /// <value>The type of the device.</value>
        public byte DeviceType => (byte)(Id >> 16 & 0x0000007F);
        /// <summary>Gets the device number.</summary>
        /// <value>The device number.</value>
        public uint DeviceNumber => (Id & 0x0000FFFF) + (Id >> 12 & 0x000F0000);
        /// <summary>Gets a value indicating whether this instance has the pairing bit set.</summary>
        /// <value>
        ///   <c>true</c> if this instance has pairing bit set; otherwise, <c>false</c>.</value>
        public bool IsPairingBitSet => (Id & 0x00800000) == 0x00800000;
        /// <summary>Gets the type of the transmission.</summary>
        /// <value>The type of the transmission.</value>
        public ChannelSharing TransmissionType => (ChannelSharing)(Id >> 24 & 0x00000003);
        /// <summary>Gets a value indicating whether global data pages are used.</summary>
        /// <value>
        ///   <c>true</c> if global data pages are used; otherwise, <c>false</c>.</value>
        public bool AreGlobalDataPagesUsed => (Id & 0x04000000) == 0x04000000;

        /// <summary>Initializes a new instance of the <see cref="ChannelId" /> struct.</summary>
        /// <param name="channelId">The channel identifier.</param>
        public ChannelId(uint channelId)
        {
            Id = channelId;
        }
    }
}
