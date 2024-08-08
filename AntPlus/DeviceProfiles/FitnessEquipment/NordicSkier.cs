using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;

namespace SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This class supports Nordic skier fitness equipment type.
    /// </summary>
    public partial class NordicSkier : FitnessEquipment
    {
        private bool isFirstDataMessage = true;
        private byte prevStride;

        /// <summary>Initializes a new instance of the <see cref="NordicSkier" /> class.</summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, int)"/>
        public NordicSkier(ChannelId channelId, IAntChannel antChannel, ILogger<NordicSkier> logger, int timeout)
            : base(channelId, antChannel, logger, timeout)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="NordicSkier" /> class.</summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, TimeoutOptions?)"/>
        public NordicSkier(ChannelId channelId, IAntChannel antChannel, ILogger<NordicSkier> logger, TimeoutOptions? options)
            : base(channelId, antChannel, logger, options)
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
        [ObservableProperty]
        private int strideCount;
        /// <summary>Gets the cadence in strides per minute.</summary>
        [ObservableProperty]
        private byte cadence;
        /// <summary>Gets the instantaneous power in watts.</summary>
        [ObservableProperty]
        private int instantaneousPower;
        /// <summary>Gets the Nordic skier specific capabilities.</summary>
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
    }
}
