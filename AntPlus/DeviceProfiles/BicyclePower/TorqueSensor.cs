using System;

namespace AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// This class supports messages common to crank/wheel torque sensors.
    /// </summary>
    /// <seealso cref="AntPlus.DeviceProfiles.BicyclePower.StandardPowerSensor" />
    public abstract class TorqueSensor : StandardPowerSensor
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

        public new byte InstantaneousCadence { get; private set; }
        public double AverageAngularVelocity { get; private set; }
        public double AverageTorque { get; private set; }
        public new double AveragePower { get; private set; }

        protected TorqueSensor(BicyclePower bp) : base(bp)
        {
        }

        public virtual void ParseTorque(byte[] dataPage)
        {
            InstantaneousCadence = dataPage[3];
            RaisePropertyChange(nameof(InstantaneousCadence));

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

                AverageAngularVelocity = Utils.ComputeAvgAngularVelocity(deltaEventCount, deltaPeriod);
                AverageTorque = Utils.ComputeAvgTorque(deltaTorque, deltaEventCount);
                AveragePower = AverageTorque * AverageAngularVelocity;
                RaisePropertyChange(nameof(AverageAngularVelocity));
                RaisePropertyChange(nameof(AverageTorque));
                RaisePropertyChange(nameof(AveragePower));
            }
        }
    }
}
