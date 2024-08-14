using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;

namespace SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This class supports the climber fitness equipment type.
    /// </summary>
    public partial class Climber : FitnessEquipment
    {
        private bool isFirstDataMessage = true;
        private byte prevStride;

        /// <summary>Initializes a new instance of the <see cref="Climber" /> class.</summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, int)"/>
        public Climber(ChannelId channelId, IAntChannel antChannel, ILogger<Climber> logger, int timeout)
            : base(channelId, antChannel, logger, timeout)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="Climber" /> class.</summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, TimeoutOptions?)"/>
        public Climber(ChannelId channelId, IAntChannel antChannel, ILogger<Climber> logger, TimeoutOptions? timeoutOptions)
            : base(channelId, antChannel, logger, timeoutOptions)
        {
        }

        /// <summary>Climber specific capabilities.</summary>
        public enum CapabilityFlags
        {
            /// <summary>Accumulated strides are not transmitted.</summary>
            None = 0x00,
            /// <summary>Transmits accumulated strides.</summary>
            TxStrides = 0x01,
        }

        /// <summary>Gets the stride cycles. Accumulated value of the complete number of stride cycles (i.e. number of steps climbed/2)</summary>
        [ObservableProperty]
        private int strideCycles;
        /// <summary>Gets the cadence in stride cycles per minute.</summary>
        [ObservableProperty]
        private byte cadence;
        /// <summary>Gets the instantaneous power in watts.</summary>
        [ObservableProperty]
        private int instantaneousPower;
        /// <summary>Gets the climber specific capabilities.</summary>
        /// <value>The capabilities.</value>
        [ObservableProperty]
        private CapabilityFlags capabilities;

        /// <summary>Parses the specified data page.</summary>
        /// <param name="dataPage">The data page.</param>
        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);
            if (handledPage) return;

            if ((DataPage)dataPage[0] == DataPage.ClimberData)
            {
                HandleFEState(dataPage[7]);
                if (isFirstDataMessage)
                {
                    isFirstDataMessage = false;
                    prevStride = dataPage[3];
                }
                else
                {
                    StrideCycles += Utils.CalculateDelta(dataPage[3], ref prevStride);
                }
                Cadence = dataPage[4];
                InstantaneousPower = BitConverter.ToUInt16(dataPage, 5);
                Capabilities = (CapabilityFlags)(dataPage[7] & 0x01);
            }
            else
            {
                CommonDataPages.ParseCommonDataPage(dataPage);
            }
        }

        /// <inheritdoc />
        public override string ToString() => "Climber";
    }
}
