using System;

namespace SmallEarthTech.AntPlus
{
    /// <summary>
    /// A static class with useful methods for calculations.
    /// </summary>
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

        /// <summary>Computes the average angular velocity.</summary>
        /// <param name="deltaEventCount">The delta event count.</param>
        /// <param name="deltaPeriod">The delta period.</param>
        /// <returns>Average angular velocity in radians per second.</returns>
        public static double ComputeAvgAngularVelocity(int deltaEventCount, int deltaPeriod)
        {
            return 2 * Math.PI * deltaEventCount / (deltaPeriod / 2048.0);
        }

        /// <summary>Computes the average torque over the event count interval.</summary>
        /// <param name="deltaTorque">The delta torque.</param>
        /// <param name="deltaEventCount">The delta event count.</param>
        /// <returns>The average torque in Nm.</returns>
        public static double ComputeAvgTorque(int deltaTorque, int deltaEventCount)
        {
            return deltaTorque / (32.0 * deltaEventCount);
        }

        /// <summary>Computes the average speed.</summary>
        /// <param name="wheelCircumference">The wheel circumference in meters.</param>
        /// <param name="deltaEventCount">The delta event count.</param>
        /// <param name="deltaPeriod">The delta period.</param>
        /// <returns>The average speed in kilometers per hour.</returns>
        public static double ComputeAvgSpeed(double wheelCircumference, int deltaEventCount, int deltaPeriod)
        {
            return (3600.0 / 1000.0) * wheelCircumference * deltaEventCount / (deltaPeriod / 2048.0);
        }

        /// <summary>Computes the change in distance.</summary>
        /// <param name="wheelCircumference">The wheel circumference in meters.</param>
        /// <param name="deltaTicks">The delta ticks.</param>
        /// <returns>Distance change in meters.</returns>
        public static double ComputeDeltaDistance(double wheelCircumference, int deltaTicks)
        {
            return wheelCircumference * deltaTicks;
        }

        /// <summary>Rotates a 16 bit number to the left.</summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="rotate">The number of bits to rotate.</param>
        /// <returns>The rotated value.</returns>
        public static ushort RotateLeft(ushort value, int rotate)
        {
            return (ushort)((value << rotate) | (value >> (16 - rotate)));
        }

        /// <summary>Rotates a 16 bit number to the right.</summary>
        /// <param name="value">The value to rotate.</param>
        /// <param name="rotate">The number of bits to rotate.</param>
        /// <returns>The rotated value.</returns>
        public static ushort RotateRight(ushort value, int rotate)
        {
            return (ushort)((value >> rotate) | (value << (16 - rotate)));
        }

        /// <summary>Convert semicircles to decimal degrees.</summary>
        /// <param name="semicircles">The semicircles.</param>
        /// <returns>Decimal degrees.</returns>
        /// <remarks>
        /// The conversion formula is decimal degrees = 180 * semicircles / 2^31.
        /// </remarks>
        public static double SemicirclesToDegrees(int semicircles)
        {
            return 180.0 * semicircles / 0x80000000;
        }

        /// <summary>Convert decimal degrees to semicircles.</summary>
        /// <param name="degrees">Decimal degrees.</param>
        /// <returns>Semicircles.</returns>
        /// <remarks>
        /// The conversion formula is semicircles = decimal degrees * 2^31 / 180.
        /// </remarks>
        public static int DegreesToSemicircles(double degrees)
        {
            return (int)(degrees * 0x80000000 / 180.0);
        }
    }
}
