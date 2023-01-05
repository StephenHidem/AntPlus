using System;

namespace AntPlus.DeviceProfiles.BicyclePower
{
    public class StandardWheelTorqueSensor : TorqueSensor
    {
        public event EventHandler WheelTorquePageChanged;

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

        public StandardWheelTorqueSensor(BicyclePower bp) : base(bp)
        {
        }

        public override void ParseTorque(byte[] dataPage)
        {
            bool firstPage = isFirstDataMessage; // save first message flag for later use
            base.ParseTorque(dataPage);
            if (!firstPage)
            {
                AverageSpeed = Utils.ComputeAvgSpeed(WheelCircumference, deltaEventCount, deltaPeriod);
                AccumulatedDistance += Utils.ComputeDeltaDistance(WheelCircumference, deltaTicks);
            }
            WheelTorquePageChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
