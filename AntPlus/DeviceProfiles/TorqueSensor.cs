using AntRadioInterface;
using System;

namespace AntPlus.DeviceProfiles
{
    public class TorqueSensor : AntDevice
    {
        private byte lastTicks;
        private byte lastEventCount;
        private ushort lastPeriod;
        private ushort lastTorque;
        private int previousAccumulatedEventCount;
        private int previousAccumulatedPeriod;
        private int previousAccumulatedTorque;

        public byte InstantaneousCadence { get; private set; }
        public int AccumulatedEventCount { get; private set; }
        public int AccumulatedTicks { get; private set; }
        public int AccumulatedPeriod { get; private set; }
        public int AccumulatedTorque { get; private set; }
        public double AverageAngularVelocity { get; private set; }
        public double AverageTorque { get; private set; }
        public double AveragePower { get; private set; }

        public TorqueSensor(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
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
                AccumulatedTicks += CalculateDelta(dataPage[2], ref lastTicks);
                InstantaneousCadence = dataPage[3];
                AccumulatedPeriod += CalculateDelta(BitConverter.ToUInt16(dataPage, 4), ref lastPeriod);
                AccumulatedTorque += CalculateDelta(BitConverter.ToUInt16(dataPage, 6), ref lastTorque);

                if (isFirstDataMessage)
                {
                    isFirstDataMessage = false;
                    return;
                }
                AverageAngularVelocity = ComputeAveAngularVelocity();
                AverageTorque = ComputeAveTorque();
                AveragePower = AverageTorque * AverageAngularVelocity;
                previousAccumulatedEventCount = AccumulatedEventCount;
                previousAccumulatedPeriod = AccumulatedPeriod;
                previousAccumulatedTorque = AccumulatedTorque;
            }
        }

        private double ComputeAveAngularVelocity()
        {
            return 2 * Math.PI * (AccumulatedEventCount - previousAccumulatedEventCount) / ((AccumulatedPeriod - previousAccumulatedPeriod) / 2048.0);
        }

        private double ComputeAveTorque()
        {
            return (AccumulatedTorque - previousAccumulatedTorque) / (32 * (AccumulatedEventCount - previousAccumulatedEventCount));
        }

    }
}
