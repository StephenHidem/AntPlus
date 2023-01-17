using System;
using System.ComponentModel;

namespace AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// Measurement data reported during calibration.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class MeasurementOutputData : INotifyPropertyChanged
    {
        /// <summary>
        /// Data type of the measurement reported.
        /// </summary>
        public enum DataType
        {
            ProgressCountdown = 0,
            TimeCountdown = 1,
            WholeSensorTorque = 8,
            LeftTorque = 9,
            RightTorque = 10,
            TorqueYAxis = 11,
            TorqueOutboardness = 12,
            WholeSensorForce = 16,
            LeftForce = 17,
            RightForce = 18,
            CrankAngle = 20,
            LeftCrankAngle = 21,
            RightCrankAngle = 22,
            ZeroOffset = 24,
            Temperature = 25,
            Voltage = 26,
            LeftForceForward = 32,
            RightForceForward = 33,
            LeftForceDownward = 34,
            RightForceDownward = 35,
            LeftPedalAngle = 40,
            RightPedalAngle = 41
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int NumberOfMeasurementTypes { get; }
        public DataType MeasurementType { get; }
        public double Timestamp { get; private set; }
        public double Measurement { get; private set; }

        internal MeasurementOutputData(byte[] dataPage)
        {
            NumberOfMeasurementTypes = dataPage[1];
            MeasurementType = (DataType)dataPage[2];
        }

        internal void Parse(byte[] dataPage)
        {
            Timestamp = BitConverter.ToUInt16(dataPage, 4) / 2048.0;
            Measurement = BitConverter.ToInt16(dataPage, 6) * Math.Pow(2, (sbyte)dataPage[3]);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Timestamp)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Measurement)));
        }
    }
}
