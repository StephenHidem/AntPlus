using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// This class supports messages common to <see cref="StandardCrankTorqueSensor"/> and <see cref="StandardWheelTorqueSensor"/>.
    /// </summary>
    public abstract partial class TorqueSensor : ObservableObject
    {
        /// <summary>
        /// Indicates whether the current data message is the first in a sequence.
        /// </summary>
        /// <remarks>This field is used to track the state of accumulated values. It is initialized to
        /// <see langword="true"/> and should be updated as data messages are processed.</remarks>
        protected bool isFirstDataMessage = true;     // used for accumulated values
        private byte lastTicks;
        /// <summary>
        /// Represents the difference in ticks used for internal calculations.
        /// </summary>
        /// <remarks>This field is intended for use within derived classes to store or manipulate
        /// tick-based values.</remarks>
        protected int deltaTicks;
        private byte lastEventCount;
        /// <summary>
        /// Represents the change in the number of events.
        /// </summary>
        /// <remarks>This field is intended for use in derived classes to track or calculate the
        /// difference in event counts. It is protected and should be accessed or modified only within the class
        /// hierarchy.</remarks>
        protected int deltaEventCount;
        private ushort lastPeriod;
        /// <summary>
        /// Represents the time interval, in milliseconds, used for calculating changes or updates.
        /// </summary>
        /// <remarks>This field is intended to be used by derived classes to define or adjust the period
        /// for specific operations. The value should be a positive integer to ensure proper functionality.</remarks>
        protected int deltaPeriod;
        private ushort lastTorque;
        /// <summary>
        /// Represents the change in torque applied to the system.
        /// </summary>
        /// <remarks>This field is intended to be used by derived classes to calculate or adjust
        /// torque-related operations. The value represents the difference in torque, typically measured in units
        /// relevant to the system's context.</remarks>
        protected int deltaTorque;

        /// <summary>
        /// Represents the logger instance used for logging messages within the class.
        /// </summary>
        /// <remarks>This protected field allows derived classes to log messages using the configured <see
        /// cref="ILogger"/> instance. Ensure that the logger is properly initialized before use to avoid runtime
        /// errors.</remarks>
        protected ILogger _logger;

        /// <summary>Gets the instantaneous pedaling cadence. 0xFF indicates invalid.</summary>
        [ObservableProperty]
        private byte instantaneousCadence;
        /// <summary>Gets the average angular velocity in radians per second.</summary>
        [ObservableProperty]
        private double averageAngularVelocity;
        /// <summary>Gets the average torque in Nm.</summary>
        [ObservableProperty]
        private double averageTorque;
        /// <summary>Gets the average power in watts.</summary>
        [ObservableProperty]
        private double averagePower;

        /// <summary>
        /// Initializes a new instance of the <see cref="TorqueSensor"/> class.
        /// </summary>
        /// <param name="logger">The logger instance used to log diagnostic and operational messages.</param>
        protected TorqueSensor(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>Parses the torque message.</summary>
        /// <param name="dataPage">The data page.</param>
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

                AverageAngularVelocity = Utils.ComputeAvgAngularVelocity(deltaEventCount, deltaPeriod);
                AverageTorque = Utils.ComputeAvgTorque(deltaTorque, deltaEventCount);
                AveragePower = AverageTorque * AverageAngularVelocity;
            }
        }
    }
}
