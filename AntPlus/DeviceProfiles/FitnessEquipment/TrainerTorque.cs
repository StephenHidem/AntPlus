using System;
using System.ComponentModel;

namespace AntPlus.DeviceProfiles.FitnessEquipment
{
    public class TrainerTorque : INotifyPropertyChanged
    {
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
        public double AverageAngularVelocity { get; private set; }
        public double AverageTorque { get; private set; }
        public double AveragePower { get; private set; }
        /// <summary>
        /// Average speed in kilometers per hour.
        /// </summary>
        public double AverageSpeed { get; private set; }
        /// <summary>
        /// Distance in meters.
        /// </summary>
        public double AccumulatedDistance { get; private set; }

        public void ParseTorque(byte[] dataPage)
        {
            bool firstPage = isFirstDataMessage; // save first message flag for later use

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
            }

            if (!firstPage)
            {
                AverageSpeed = Utils.ComputeAvgSpeed(WheelCircumference, deltaEventCount, deltaPeriod);
                AccumulatedDistance += Utils.ComputeDeltaDistance(WheelCircumference, deltaTicks);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}
