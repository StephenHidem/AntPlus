﻿namespace DeviceProfiles
{
    public class StandardWheelTorqueSensor : TorqueSensor
    {
        /// <summary>
        /// Wheel circumference in meters. The default is 2.2 meters.
        /// </summary>
        public double WheelCircumference { get; set; } = 2.2;
        /// <summary>
        /// Average speed in kilometers per hour.
        /// </summary>
        public double AverageSpeed { get; private set; }
        /// <summary>
        /// Distance in meters.
        /// </summary>
        public double AccumulatedDistance { get; private set; }

        public override void Parse(byte[] dataPage)
        {
            bool firstPage = isFirstDataMessage; // save first message flag for later use
            base.Parse(dataPage);
            if (!firstPage)
            {
                AverageSpeed = ComputeAvgSpeed();
                AccumulatedDistance += ComputeDeltaDistance();
            }
        }

        private double ComputeAvgSpeed()
        {
            return (3600.0 / 1000.0) * WheelCircumference * deltaEventCount / (deltaPeriod / 2048.0);
        }

        private double ComputeDeltaDistance()
        {
            return WheelCircumference * deltaTicks;
        }
    }
}
