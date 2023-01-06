using System;

namespace AntPlus
{
    public static class Utils
    {
        /// <summary>
        /// Calculates the delta of the current and previous values. Rollover is accounted for and a positive integer is always returned.
        /// Add the returned value to the accumulated value in the derived class. The last value is updated with the current value.
        /// </summary>
        /// <param name="currentValue">The current value.</param>
        /// <param name="lastValue">The last value.</param>
        /// <returns>Positive delta of the current and previous values.</returns>
        public static int CalculateDelta(byte currentValue, ref byte lastValue)
        {
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
        public static int CalculateDelta(ushort currentValue, ref ushort lastValue)
        {
            int delta = currentValue - lastValue;
            if (lastValue > currentValue)
            {
                // rollover
                delta += 0x10000;
            }

            lastValue = currentValue;
            return delta;
        }

        public static double ComputeAvgAngularVelocity(int deltaEventCount, int deltaPeriod)
        {
            return 2 * Math.PI * deltaEventCount / (deltaPeriod / 2048.0);
        }

        public static double ComputeAvgTorque(int deltaTorque, int deltaEventCount)
        {
            return deltaTorque / (32.0 * deltaEventCount);
        }

        public static double ComputeAvgSpeed(double wheelCircumference, int deltaEventCount, int deltaPeriod)
        {
            return (3600.0 / 1000.0) * wheelCircumference * deltaEventCount / (deltaPeriod / 2048.0);
        }

        public static double ComputeDeltaDistance(double wheelCircumference, int deltaTicks)
        {
            return wheelCircumference * deltaTicks;
        }

        public static ushort RotateLeft(ushort value, int rotate)
        {
            return (ushort)((value << rotate) | (value >> (16 - rotate)));
        }

        public static ushort RotateRight(ushort value, int rotate)
        {
            return (ushort)((value >> rotate) | (value << (16 - rotate)));
        }
    }
}
