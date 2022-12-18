using AntRadioInterface;
using System;

namespace AntPlus.DeviceProfiles.BikeSpeedAndCadence
{
    internal class CombinedSpeedAndCadenceSensor : AntDevice
    {
        /// <summary>
        /// The CombinedSpeedAndCadenceSensor device class ID.
        /// </summary>
        public const byte DeviceClass = 121;

        private bool isFirstDataMessage = true;     // used for accumulated values
        private ushort prevEventTime;
        private bool pageToggle = false;
        private int observedToggle;

        public CombinedSpeedAndCadenceSensor(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
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
