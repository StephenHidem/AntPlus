﻿using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;

namespace SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This class supports the rower fitness equipment type.
    /// </summary>
    public partial class Rower : FitnessEquipment
    {
        private bool isFirstDataMessage = true;
        private byte prevStroke;

        /// <summary>Initializes a new instance of the <see cref="Rower" /> class.</summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        /// <param name="logger">Logger to use.</param>
        /// <param name="timeout">Time in milliseconds before firing <see cref="AntDevice.DeviceWentOffline" />.</param>
        public Rower(ChannelId channelId, IAntChannel antChannel, ILogger<FitnessEquipment> logger, int timeout = 2000) : base(channelId, antChannel, logger, timeout)
        {
        }

        /// <summary>Rower specific capabilities.</summary>
        public enum CapabilityFlags
        {
            /// <summary>No supported capabilities.</summary>
            None,
            /// <summary>Transmits accumulated stroke count.</summary>
            TxStrokeCount = 0x01,
        }

        /// <summary>Gets the accumulated stroke count.</summary>
        [ObservableProperty]
        private int strokeCount;
        /// <summary>Gets the cadence in strokes per minute.</summary>
        [ObservableProperty]
        private byte cadence;
        /// <summary>Gets the instantaneous power in watts.</summary>
        [ObservableProperty]
        private int instantaneousPower;
        /// <summary>Gets the rower specific capabilities.</summary>
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
            if ((DataPage)dataPage[0] == DataPage.RowerData)
            {
                HandleFEState(dataPage[7]);
                if (isFirstDataMessage)
                {
                    isFirstDataMessage = false;
                    prevStroke = dataPage[3];
                }
                else
                {
                    StrokeCount += Utils.CalculateDelta(dataPage[3], ref prevStroke);
                }
                Cadence = dataPage[4];
                InstantaneousPower = BitConverter.ToUInt16(dataPage, 5);
                Capabilities = (CapabilityFlags)(dataPage[7] & 0x01);
            }
        }

        /// <inheritdoc />
        public override string ToString() => "Rower";
    }
}
