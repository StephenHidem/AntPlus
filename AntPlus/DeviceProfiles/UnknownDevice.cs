using AntRadioInterface;
using System;

namespace AntPlus.DeviceProfiles
{
    public class UnknownDevice : AntDevice
    {
        public byte[] DataPage;

        public UnknownDevice(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
        }

        public override void Parse(byte[] dataPage)
        {
            RaisePropertyChange(nameof(DataPage));
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
