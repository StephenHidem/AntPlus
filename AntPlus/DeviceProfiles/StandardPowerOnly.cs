using System;

namespace AntPlus.DeviceProfiles
{
    public class StandardPowerOnly
    {
        protected bool isFirstDataMessage = true;     // used for accumulated values
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

        public void Parse(byte[] dataPage)
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

        /// <summary>
        /// Calculates the delta of the current and previous values. Rollover is accounted for and a positive integer is always returned.
        /// Add the returned value to the accumulated value in the derived class. The last value is updated with the current value.
        /// </summary>
        /// <param name="currentValue">The current value.</param>
        /// <param name="lastValue">The last value.</param>
        /// <returns>Positive delta of the current and previous values.</returns>
        private int CalculateDelta(byte currentValue, ref byte lastValue)
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
        private int CalculateDelta(ushort currentValue, ref ushort lastValue)
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
