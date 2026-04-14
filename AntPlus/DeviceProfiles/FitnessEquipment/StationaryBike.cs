using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;

namespace SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This class supports the ANT+ Legacy Stationary Bike device profile. It extends the FitnessEquipment base class to provide specific properties and parsing logic for stationary bike data pages, including cadence and instantaneous power.
    /// </summary>
    public partial class StationaryBike : FitnessEquipment
    {
        /// <summary>Gets the instantaneous pedaling cadence in revolutions per minute.</summary>
        /// <value>The instantaneous cadence. 255 (0xFF) indicates invalid.</value>
        [ObservableProperty]
        private byte cadence;
        /// <summary>Gets the instantaneous power in watts.</summary>
        /// <value>The instantaneous power. 65535 (0xFFFF) indicates invalid.</value>
        [ObservableProperty]
        private int instantaneousPower;

        /// <summary>Initializes a new instance of the <see cref="StationaryBike" /> class.</summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, int)"/>
        public StationaryBike(ChannelId channelId, IAntChannel antChannel, ILogger logger, int timeout) : base(channelId, antChannel, logger, timeout)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="StationaryBike" /> class.</summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, TimeoutOptions?)"/>
        public StationaryBike(ChannelId channelId, IAntChannel antChannel, ILogger logger, TimeoutOptions? timeoutOptions) : base(channelId, antChannel, logger, timeoutOptions)
        {
        }

        /// <summary>
        /// Parses the specified data page and updates the relevant properties.
        /// </summary>
        /// <remarks>If the data page represents stationary bike data, the method extracts cadence and
        /// instantaneous power values. For other data page types, common data page parsing is delegated to the
        /// CommonDataPages class. The method does not process the page if it has already been handled.</remarks>
        /// <param name="dataPage">A byte array containing the data page to parse.</param>
        override public void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);
            if (handledPage) return;

            if ((DataPage)dataPage[0] == DataPage.StationaryBikeData)
            {
                HandleFEState(dataPage);
                Cadence = dataPage[4];
                InstantaneousPower = BitConverter.ToInt16(dataPage, 5);
            }
            else
            {
                CommonDataPages.ParseCommonDataPage(dataPage);
            }
        }

        /// <inheritdoc/>
        public override string ToString() => "Stationary Bike";
    }
}
