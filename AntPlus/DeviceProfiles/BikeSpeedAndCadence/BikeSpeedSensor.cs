using AntRadioInterface;
using System;

namespace AntPlus.DeviceProfiles.BikeSpeedAndCadence
{
    internal class BikeSpeedSensor : CommonSpeedCadence
    {
        /// <summary>
        /// The BikeSpeedSensor device class ID.
        /// </summary>
        public const byte DeviceClass = 123;


        public event EventHandler BikeSpeedSensorChanged;

        public double WheelCircumference { get; set; } = 2.2;
        public double InstantaneousSpeed { get; private set; }
        public double AccumulatedDistance { get; private set; }

        public BikeSpeedSensor(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
        }

        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);
            if (!isFirstDataMessage)
            {
                // this data is present in all data pages
                int deltaEventTime = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 4), ref prevEventTime);
                int deltaRevCount = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 6), ref prevRevCount);
                InstantaneousSpeed = WheelCircumference * deltaRevCount * 1024.0 / deltaEventTime;
                AccumulatedDistance = WheelCircumference * deltaRevCount;
                BikeSpeedSensorChanged?.Invoke(this, EventArgs.Empty);
            }
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
