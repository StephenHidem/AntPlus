using AntPlus;
using System;

namespace DeviceProfiles
{
    public abstract class TorqueSensor : BicyclePowerSensor
    {
        protected bool isFirstDataMessage = true;     // used for accumulated values
        private byte lastTicks;
        protected int deltaTicks;
        private byte lastEventCount;
        protected int deltaEventCount;
        private ushort lastPeriod;
        protected int deltaPeriod;
        private ushort lastTorque;
        protected int deltaTorque;

        public byte InstantaneousCadence { get; private set; }
        public double AverageAngularVelocity { get; private set; }
        public double AverageTorque { get; private set; }
        public double AveragePower { get; private set; }

        public virtual void ParseTorque(byte[] dataPage)
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
                deltaEventCount = Utils.CalculateDelta(dataPage[1], ref lastEventCount);
                deltaTicks = Utils.CalculateDelta(dataPage[2], ref lastTicks);
                deltaPeriod = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 4), ref lastPeriod);
                deltaTorque = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 6), ref lastTorque);

                AverageAngularVelocity = ComputeAvgAngularVelocity();
                AverageTorque = ComputeAvgTorque();
                AveragePower = AverageTorque * AverageAngularVelocity;
            }
        }

        private double ComputeAvgAngularVelocity()
        {
            return 2 * Math.PI * deltaEventCount / (deltaPeriod / 2048.0);
        }

        private double ComputeAvgTorque()
        {
            return deltaTorque / (32.0 * deltaEventCount);
        }
    }
}
