using AntPlus;
using System;

namespace DeviceProfiles
{
    public class UnknownDevice : AntDevice
    {
        public event EventHandler<byte[]> DeviceChanged;

        public UnknownDevice(ChannelId channelId) : base(channelId)
        {
        }

        public override void Parse(byte[] dataPage)
        {
            DeviceChanged?.Invoke(this, dataPage);
        }
    }
}
