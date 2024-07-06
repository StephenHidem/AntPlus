using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// This class supports messages common to crank/wheel torque sensors.
    /// </summary>
    public abstract partial class TorqueSensor : ObservableObject
    {
        /// <summary>The is first data message
        /// received</summary>
        protected bool isFirstDataMessage = true;     // used for accumulated values
        private byte lastTicks;
        /// <summary>The delta ticks</summary>
        protected int deltaTicks;
        private byte lastEventCount;
        /// <summary>The delta event count</summary>
        protected int deltaEventCount;
        private ushort lastPeriod;
        /// <summary>The delta period</summary>
        protected int deltaPeriod;
        private ushort lastTorque;
        /// <summary>The delta torque</summary>
        protected int deltaTorque;

        /// <summary>The logger</summary>
        protected ILogger _logger;
        /// <summary>The standard power sensor.</summary>
        protected StandardPowerSensor _powerSensor;

        /// <summary>Gets the average angular velocity in radians per second.</summary>
        [ObservableProperty]
        private double averageAngularVelocity;
        /// <summary>Gets the average torque in Nm.</summary>
        [ObservableProperty]
        private double averageTorque;
        /// <summary>Gets the average power in watts.</summary>
        [ObservableProperty]
        private double averagePower;

        /// <summary>Initializes a new instance of the <see cref="TorqueSensor" /> class.</summary>
        /// <param name="sensor">The <see cref="StandardPowerSensor"/>.</param>
        /// <param name="logger">The logger.</param>
        protected TorqueSensor(StandardPowerSensor sensor, ILogger logger)
        {
            _powerSensor = sensor;
            _logger = logger;
        }

        /// <summary>Parses the torque message.</summary>
        /// <param name="dataPage">The data page.</param>
        public virtual void ParseTorque(byte[] dataPage)
        {
            _powerSensor.InstantaneousCadence = dataPage[3];
            _powerSensor.RaisePropertyChange(nameof(_powerSensor.InstantaneousCadence));

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
