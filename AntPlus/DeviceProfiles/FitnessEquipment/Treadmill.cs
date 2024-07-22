using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;

namespace SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This class supports the treadmill fitness equipment type.
    /// </summary>
    public partial class Treadmill : FitnessEquipment
    {
        private bool isFirstDataMessage = true;
        private byte prevPos;
        private byte prevNeg;

        /// <summary>Initializes a new instance of the <see cref="Treadmill" /> class.</summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        /// <param name="logger">Logger to use.</param>
        /// <param name="timeout">Time in milliseconds before firing <see cref="AntDevice.DeviceWentOffline" />.</param>
        public Treadmill(ChannelId channelId, IAntChannel antChannel, ILogger logger, int timeout)
            : base(channelId, antChannel, logger, timeout)
        {
        }

        /// <param name="missedMessages">The number of missed messages before signaling the device went offline.</param>
        /// <inheritdoc cref="Treadmill(ChannelId, IAntChannel, ILogger, int)" />
        public Treadmill(ChannelId channelId, IAntChannel antChannel, ILogger logger, byte missedMessages)
            : base(channelId, antChannel, logger, missedMessages)
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
        [ObservableProperty]
        private byte cadence;
        /// <summary>Gets the accumulated negative vertical distance traveled in meters. Note this is a negative value.</summary>
        [ObservableProperty]
        private double negVerticalDistance;
        /// <summary>Gets the accumulated positive vertical distance traveled in meters.</summary>
        [ObservableProperty]
        private double posVerticalDistance;
        /// <summary>Gets the treadmill specific capabilities.</summary>
        /// <value>The capabilities.</value>
        [ObservableProperty]
        private CapabilityFlags capabilities;

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);
            if (handledPage) return;

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
                }
                Cadence = dataPage[4];
                Capabilities = (CapabilityFlags)(dataPage[7] & 0x03);
            }
            else
            {
                CommonDataPages.ParseCommonDataPage(dataPage);
            }
        }

        /// <inheritdoc />
        public override string ToString() => "Treadmill";
    }
}
