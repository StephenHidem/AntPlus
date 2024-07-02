using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;

namespace SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This class supports the treadmill fitness equipment type.
    /// </summary>
    public class Treadmill : Equipment
    {
        private bool isFirstDataMessage = true;
        private byte prevPos;
        private byte prevNeg;

        /// <summary>Initializes a new instance of the <see cref="Treadmill" /> class.</summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        /// <param name="logger">Logger to use.</param>
        /// <param name="timeout">Time in milliseconds before firing <see cref="AntDevice.DeviceWentOffline" />.</param>
        public Treadmill(ChannelId channelId, IAntChannel antChannel, ILogger<Equipment> logger, int timeout = 2000) : base(channelId, antChannel, logger, timeout)
        {
        }

        /// <summary>Treadmill specific capabilities.</summary>
        [Flags]
        public enum CapabilityFlags
        {
            /// <summary>Positive and negative vertical distance is not transmitted.</summary>
            None = 0x00,
            /// <summary>Transmits positive vertical distance.</summary>
            TxPosVertDistance = 0x01,
            /// <summary>Transmits negative vertical distance.</summary>
            TxNegVertDistance = 0x02
        }

        /// <summary>Gets the cadence in strides per minute.</summary>
        public byte Cadence { get; private set; }
        /// <summary>Gets the accumulated negative vertical distance traveled in meters. Note this is a negative value.</summary>
        public double NegVerticalDistance { get; private set; }
        /// <summary>Gets the accumulated positive vertical distance traveled in meters.</summary>
        public double PosVerticalDistance { get; private set; }
        /// <summary>Gets the treadmill specific capabilities.</summary>
        /// <value>The capabilities.</value>
        public CapabilityFlags Capabilities { get; private set; }

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public override void Parse(byte[] dataPage)
        {
            if ((DataPage)dataPage[0] == DataPage.TreadmillData)
            {
                if (isFirstDataMessage)
                {
                    prevNeg = dataPage[5];
                    prevPos = dataPage[6];
                    isFirstDataMessage = false;
                }
                else
                {
                    NegVerticalDistance += Utils.CalculateDelta(dataPage[5], ref prevNeg) / -10.0;
                    PosVerticalDistance += Utils.CalculateDelta(dataPage[6], ref prevPos) / 10.0;
                    RaisePropertyChange(nameof(NegVerticalDistance));
                    RaisePropertyChange(nameof(PosVerticalDistance));
                }
                Cadence = dataPage[4];
                Capabilities = (CapabilityFlags)(dataPage[7] & 0x03);
                RaisePropertyChange(nameof(Cadence));
                RaisePropertyChange(nameof(Capabilities));
            }
            else
            {
                base.Parse(dataPage);
            }
        }

        /// <inheritdoc />
        public override string ToString() => "Treadmill";
    }
}
