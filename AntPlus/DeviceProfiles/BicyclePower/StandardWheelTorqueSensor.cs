using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// The standard wheel torque sensor.
    /// </summary>
    /// <remarks>
    /// Set the wheel circumference if the default value is incorrect. The calculations rely
    /// on the wheel circumference.
    /// </remarks>
    /// <seealso cref="TorqueSensor" />
    public partial class StandardWheelTorqueSensor : TorqueSensor
    {
        /// <summary>
        /// Wheel circumference in meters. The default is 2.2 meters.
        /// </summary>
        [ObservableProperty]
        private double wheelCircumference = 2.2;
        /// <summary>
        /// Average speed in kilometers per hour.
        /// </summary>
        [ObservableProperty]
        private double averageSpeed;
        /// <summary>
        /// Accumulated distance in meters.
        /// </summary>
        [ObservableProperty]
        private double accumulatedDistance;

        /// <summary>Initializes a new instance of the <see cref="StandardWheelTorqueSensor" /> class.</summary>
        /// <param name="sensor">The standard power sensor.</param>
        /// <param name="logger">The logger to use.</param>
        public StandardWheelTorqueSensor(StandardPowerSensor sensor, ILogger logger) : base(sensor, logger)
        {
        }

        /// <inheritdoc/>
        public override void ParseTorque(byte[] dataPage)
        {
            base.ParseTorque(dataPage);
            if (deltaEventCount != 0)
            {
                AverageSpeed = Utils.ComputeAvgSpeed(WheelCircumference, deltaEventCount, deltaPeriod);
                AccumulatedDistance += Utils.ComputeDeltaDistance(WheelCircumference, deltaTicks);
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Bike Power (Wheel Torque)";
        }
    }
}
