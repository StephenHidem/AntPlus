using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;

namespace SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This class supports the elliptical fitness equipment type.
    /// </summary>
    public partial class Elliptical : Equipment
    {
        private bool isFirstDataMessage = true;
        private byte prevPos;
        private byte prevStride;

        /// <summary>Initializes a new instance of the <see cref="Elliptical" /> class.</summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        /// <param name="logger">Logger to use.</param>
        /// <param name="timeout">Time in milliseconds before firing <see cref="AntDevice.DeviceWentOffline" />.</param>
        public Elliptical(ChannelId channelId, IAntChannel antChannel, ILogger<Equipment> logger, int timeout = 2000) : base(channelId, antChannel, logger, timeout)
        {
        }

        /// <summary>Elliptical specific capabilities.</summary>
        [Flags]
        public enum CapabilityFlags
        {
            /// <summary>No supported capabilities.</summary>
            None = 0x00,
            /// <summary>Transmits stride count.</summary>
            TxStrideCount = 0x01,
            /// <summary>Transmits positive vertical distance.</summary>
            TxPosVertDistance = 0x02
        }

        /// <summary>Gets the stride count. This is an accumulated value.</summary>
        [ObservableProperty]
        private int strideCount;
        /// <summary>Gets the cadence in strides per minute.</summary>
        [ObservableProperty]
        private byte cadence;
        /// <summary>Gets the positive vertical distance in meters.</summary>
        [ObservableProperty]
        private double posVerticalDistance;
        /// <summary>Gets the instantaneous power in watts.</summary>
        [ObservableProperty]
        private int instantaneousPower;
        /// <summary>Gets the elliptical specific capabilities.</summary>
        /// <value>The capabilities.</value>
        [ObservableProperty]
        private CapabilityFlags capabilities;

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public override void Parse(byte[] dataPage)
        {
            if ((DataPage)dataPage[0] == DataPage.EllipticalData)
            {
                HandleFEState(dataPage[7]);
                if (isFirstDataMessage)
                {
                    isFirstDataMessage = false;
                    prevPos = dataPage[2];
                    prevStride = dataPage[3];
                }
                else
                {
                    PosVerticalDistance += Utils.CalculateDelta(dataPage[2], ref prevPos) / 10.0;
                    StrideCount += Utils.CalculateDelta(dataPage[3], ref prevStride);
                }
                Cadence = dataPage[4];
                InstantaneousPower = BitConverter.ToUInt16(dataPage, 5);
                Capabilities = (CapabilityFlags)(dataPage[7] & 0x03);
            }
            else { base.Parse(dataPage); }
        }

        /// <inheritdoc />
        public override string ToString() => "Elliptical";
    }
}
