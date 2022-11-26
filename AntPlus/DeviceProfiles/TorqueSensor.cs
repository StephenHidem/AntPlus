using System;

namespace AntPlus.DeviceProfiles
{
    public class TorqueSensor
    {
        protected bool isFirstDataMessage = true;     // used for accumulated values
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

        public void Parse(byte[] dataPage)
        {
            InstantaneousCadence = dataPage[3];

            if (isFirstDataMessage)
            {
                // initialize if first data message
                isFirstDataMessage = false;
                lastEventCount = dataPage[1];
                lastTicks = dataPage[2];
                lastPeriod = BitConverter.ToUInt16(dataPage, 4);
                lastTorque = BitConverter.ToUInt16(dataPage, 6);
                return;
            }

            if (dataPage[1] != lastEventCount)
            {
                // handle new events
                AccumulatedEventCount += Utils.CalculateDelta(dataPage[1], ref lastEventCount);
                AccumulatedTicks += Utils.CalculateDelta(dataPage[2], ref lastTicks);
                AccumulatedPeriod += Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 4), ref lastPeriod);
                AccumulatedTorque += Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 6), ref lastTorque);
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
            return (AccumulatedTorque - previousAccumulatedTorque) / (32.0 * (AccumulatedEventCount - previousAccumulatedEventCount));
        }
    }
}
