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

        /// <summary>
        /// Calculates the delta of the current and previous values. Rollover is accounted for and a positive integer is always returned.
        /// Add the returned value to the accumulated value in the derived class. The last value is updated with the current value.
        /// </summary>
        /// <param name="currentValue">The current value.</param>
        /// <param name="lastValue">The last value.</param>
        /// <returns>Positive delta of the current and previous values.</returns>
        protected int CalculateDelta(byte currentValue, ref byte lastValue)
        {
            if (isFirstDataMessage)
            {
                lastValue = currentValue;
                return 0;
            }

            int delta = currentValue - lastValue;
            if (lastValue > currentValue)
            {
                // rollover
                delta += 256;
            }

            lastValue = currentValue;
            return delta;
        }

        /// <summary>
        /// Calculates the delta of the current and previous values. Rollover is accounted for and a positive integer is always returned.
        /// Add the returned value to the accumulated value in the derived class. The last value is updated with the current value.
        /// </summary>
        /// <param name="currentValue">The current value.</param>
        /// <param name="lastValue">The last value.</param>
        /// <returns>Positive delta of the current and previous values.</returns>
        protected int CalculateDelta(ushort currentValue, ref ushort lastValue)
        {
            if (isFirstDataMessage)
            {
                lastValue = currentValue;
                return 0;
            }

            int delta = currentValue - lastValue;
            if (lastValue > currentValue)
            {
                // rollover
                delta += 0x10000;
            }

            lastValue = currentValue;
            return delta;
        }
    }
}
