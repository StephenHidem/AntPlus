using AntRadioInterface;
using System;

namespace AntPlus.DeviceProfiles
{
    public class StandardPowerOnly : AntDevice
    {
        private byte lastEventCount;
        private int previousAccumulatedEventCount;

        private ushort lastPower;
        private int previousAccumulatedPower;

        public int AccumulatedEventCount { get; private set; }
        public double AveragePower { get; private set; }
        public byte PedalPower { get; private set; }
        public byte InstantaneousCadence { get; private set; }
        public int AccumulatedPower { get; private set; }
        public ushort InstantaneousPower { get; private set; }

        public StandardPowerOnly(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
        }

        public override void ChannelEventHandler(EventMsgId eventMsgId)
        {
            throw new NotImplementedException();
        }

        public override void ChannelResponseHandler(byte messageId, ResponseMsgId responseMsgId)
        {
            throw new NotImplementedException();
        }

        public override void Parse(byte[] dataPage)
        {
            if (dataPage[1] != lastEventCount)
            {
                AccumulatedEventCount += CalculateDelta(dataPage[1], ref lastEventCount);
                PedalPower = dataPage[2];
                InstantaneousCadence = dataPage[3];
                AccumulatedPower += CalculateDelta(BitConverter.ToUInt16(dataPage, 4), ref lastPower);
                InstantaneousPower = BitConverter.ToUInt16(dataPage, 6);
                if (isFirstDataMessage)
                {
                    isFirstDataMessage = false;
                    return;
                }
                AveragePower = (AccumulatedPower - previousAccumulatedPower) / (AccumulatedEventCount - previousAccumulatedEventCount);
                previousAccumulatedEventCount = AccumulatedEventCount;
                previousAccumulatedPower = AccumulatedPower;
            }
        }
    }
}
