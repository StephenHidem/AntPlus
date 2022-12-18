using AntRadioInterface;
using System;

namespace AntPlus.DeviceProfiles.BikeSpeedAndCadence
{
    internal class BikeCadenceSensor : AntDevice
    {
        /// <summary>
        /// The BikeCadenceSensor device class ID.
        /// </summary>
        public const byte DeviceClass = 122;

        /// <summary>
        /// Bike cadence data pages.
        /// </summary>
        public enum DataPage
        {
            /// <summary>Default or unknown data page.</summary>
            Default,
            /// <summary>Cumulative operating time</summary>
            CumulativeOperatingTime,
            /// <summary>Manufacturer information</summary>
            ManufacturerInfo,
            /// <summary>Product information</summary>
            ProductInfo,
            /// <summary>Battery status</summary>
            BatteryStatus,
            /// <summary>Motion and cadence</summary>
            MotionAndCadence,
        }

        public BikeCadenceSensor(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
        }

        public override void Parse(byte[] dataPage)
        {
            throw new NotImplementedException();
        }

        public override void ChannelEventHandler(EventMsgId eventMsgId)
        {
            throw new NotImplementedException();
        }

        public override void ChannelResponseHandler(byte messageId, ResponseMsgId responseMsgId)
        {
            throw new NotImplementedException();
        }

    }
}
