namespace AntPlus.DeviceProfiles.BicyclePower
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

        public StandardWheelTorqueSensor(BicyclePower bp) : base(bp)
        {
        }

        public override void ParseTorque(byte[] dataPage)
        {
            base.ParseTorque(dataPage);
            if (deltaEventCount != 0)
            {
                AverageSpeed = Utils.ComputeAvgSpeed(WheelCircumference, deltaEventCount, deltaPeriod);
                AccumulatedDistance += Utils.ComputeDeltaDistance(WheelCircumference, deltaTicks);
                RaisePropertyChange(nameof(AverageSpeed));
                RaisePropertyChange(nameof(AccumulatedDistance));
            }
        }
    }
}
