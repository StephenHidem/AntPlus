using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;

namespace SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This class supports Nordic skier fitness equipment type.
    /// </summary>
    public class NordicSkier : Equipment
    {
        private bool isFirstDataMessage = true;
        private byte prevStride;

        /// <summary>Initializes a new instance of the <see cref="NordicSkier" /> class.</summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        /// <param name="logger">Logger to use.</param>
        /// <param name="timeout">Time in milliseconds before firing <see cref="AntDevice.DeviceWentOffline" />.</param>
        public NordicSkier(ChannelId channelId, IAntChannel antChannel, ILogger<Equipment> logger, int timeout = 2000) : base(channelId, antChannel, logger, timeout)
        {
        }

        /// <summary>Nordic skier specific capabilities.</summary>
        public enum CapabilityFlags
        {
            /// <summary>No supported capabilities.</summary>
            None,
            /// <summary>Transmits accumulated stride count.</summary>
            TxStrideCount = 0x01,
        }

        /// <summary>Gets the stride count.</summary>
        public int StrideCount { get; private set; }
        /// <summary>Gets the cadence in strides per minute.</summary>
        public byte Cadence { get; private set; }
        /// <summary>Gets the instantaneous power in watts.</summary>
        public int InstantaneousPower { get; private set; }
        /// <summary>Gets the Nordic skier specific capabilities.</summary>
        /// <value>The capabilities.</value>
        public CapabilityFlags Capabilities { get; private set; }

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public override void Parse(byte[] dataPage)
        {
            if ((DataPage)dataPage[0] == DataPage.NordicSkierData)
            {
                HandleFEState(dataPage[7]);
                if (isFirstDataMessage)
                {
                    isFirstDataMessage = false;
                    prevStride = dataPage[3];
                }
                else
                {
                    StrideCount += Utils.CalculateDelta(dataPage[3], ref prevStride);
                    RaisePropertyChange(nameof(StrideCount));
                }
                Cadence = dataPage[4];
                InstantaneousPower = BitConverter.ToUInt16(dataPage, 5);
                Capabilities = (CapabilityFlags)(dataPage[7] & 0x01);
                RaisePropertyChange(nameof(Cadence));
                RaisePropertyChange(nameof(InstantaneousPower));
                RaisePropertyChange(nameof(Capabilities));
            }
            else { base.Parse(dataPage); }
        }
    }
}
