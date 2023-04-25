using System;
using System.ComponentModel;

namespace SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This class supports the trainer torque messages.
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public class TrainerTorque : INotifyPropertyChanged
    {
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isFirstDataMessage = true;     // used for accumulated values
        private byte lastTicks;
        private int deltaTicks;
        private byte lastEventCount;
        private int deltaEventCount;
        private ushort lastPeriod;
        private int deltaPeriod;
        private ushort lastTorque;
        private int deltaTorque;

        /// <summary>
        /// Wheel circumference in meters. The default is 2.2 meters.
        /// </summary>
        public double WheelCircumference { get; set; } = 2.2;
        /// <summary>Gets the average angular velocity in radians per second.</summary>
        public double AverageAngularVelocity { get; private set; }
        /// <summary>Gets the average torque in Nm.</summary>
        public double AverageTorque { get; private set; }
        /// <summary>Gets the average power in watts.</summary>
        public double AveragePower { get; private set; }
        /// <summary>
        /// Average speed in kilometers per hour.
        /// </summary>
        public double AverageSpeed { get; private set; }
        /// <summary>
        /// Distance in meters.
        /// </summary>
        public double AccumulatedDistance { get; private set; }

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
                deltaEventCount = Utils.CalculateDelta(dataPage[1], ref lastEventCount);
                deltaTicks = Utils.CalculateDelta(dataPage[2], ref lastTicks);
                deltaPeriod = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 3), ref lastPeriod);
                deltaTorque = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 5), ref lastTorque);

                AverageAngularVelocity = Utils.ComputeAvgAngularVelocity(deltaEventCount, deltaPeriod);
                AverageTorque = Utils.ComputeAvgTorque(deltaTorque, deltaEventCount);
                AveragePower = AverageTorque * AverageAngularVelocity;
                AverageSpeed = Utils.ComputeAvgSpeed(WheelCircumference, deltaEventCount, deltaPeriod);
                AccumulatedDistance += Utils.ComputeDeltaDistance(WheelCircumference, deltaTicks);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
            }
        }
    }
}
