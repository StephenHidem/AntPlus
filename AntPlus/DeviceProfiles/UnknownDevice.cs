using AntRadioInterface;
using System;

namespace AntPlus.DeviceProfiles
{
    public class UnknownDevice : AntDevice
    {
        public event EventHandler<byte[]> DeviceChanged;

        public UnknownDevice(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
        }

        public override void Parse(byte[] dataPage)
        {
            DeviceChanged?.Invoke(this, dataPage);
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
