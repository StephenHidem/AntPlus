using AntRadioInterface;
using System;

namespace AntPlus.DeviceProfiles.BikeSpeedAndCadence
{
    internal class BikeCadenceSensor : CommonSpeedCadence
    {
        /// <summary>
        /// The BikeCadenceSensor device class ID.
        /// </summary>
        public const byte DeviceClass = 122;

        public event EventHandler BikeCadenceSensorChanged;

        public double InstantaneousCadence { get; private set; }

        public BikeCadenceSensor(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
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
                InstantaneousCadence = 60.0 * deltaRevCount * 1024.0 / deltaEventTime;
                BikeCadenceSensorChanged?.Invoke(this, EventArgs.Empty);
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
