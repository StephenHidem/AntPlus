using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This class supports trainer torque messages.
    /// </summary>
    public partial class TrainerTorque : ObservableObject
    {
        private bool isFirstDataMessage = true;     // used for accumulated values
        private byte lastTicks;
        private byte lastEventCount;
        private ushort lastPeriod;
        private ushort lastTorque;

        /// <summary>
        /// Wheel circumference in meters. The default is 2.2 meters.
        /// </summary>
        [ObservableProperty]
        private double wheelCircumference = 2.2;
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
        /// Average speed in kilometers per hour.
        /// </summary>
        [ObservableProperty]
        private double averageSpeed;
        /// <summary>
        /// Distance in meters.
        /// </summary>
        [ObservableProperty]
        private double accumulatedDistance;

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public void Parse(byte[] dataPage)
        {
            if (isFirstDataMessage)
            {
                // initialize if first data message
                isFirstDataMessage = false;
                lastEventCount = dataPage[1];
                lastTicks = dataPage[2];
                lastPeriod = BitConverter.ToUInt16(dataPage, 3);
                lastTorque = BitConverter.ToUInt16(dataPage, 5);
                return;
            }

            if (dataPage[1] != lastEventCount)
            {
                // handle new events
                int deltaEventCount = Utils.CalculateDelta(dataPage[1], ref lastEventCount);
                int deltaTicks = Utils.CalculateDelta(dataPage[2], ref lastTicks);
                int deltaPeriod = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 3), ref lastPeriod);
                int deltaTorque = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 5), ref lastTorque);

                AverageAngularVelocity = Utils.ComputeAvgAngularVelocity(deltaEventCount, deltaPeriod);
                AverageTorque = Utils.ComputeAvgTorque(deltaTorque, deltaEventCount);
                AveragePower = AverageTorque * AverageAngularVelocity;
                AverageSpeed = Utils.ComputeAvgSpeed(WheelCircumference, deltaEventCount, deltaPeriod);
                AccumulatedDistance += Utils.ComputeDeltaDistance(WheelCircumference, deltaTicks);
            }
        }
    }
}
