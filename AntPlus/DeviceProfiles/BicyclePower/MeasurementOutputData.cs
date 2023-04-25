﻿using System;
using System.ComponentModel;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// Measurement data reported during calibration.
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public class MeasurementOutputData : INotifyPropertyChanged
    {
        /// <summary>
        /// Data type of the measurement reported.
        /// </summary>
        public enum DataType
        {
            /// <summary>The progress countdown</summary>
            ProgressCountdown = 0,
            /// <summary>The time countdown</summary>
            TimeCountdown = 1,
            /// <summary>The whole sensor torque</summary>
            WholeSensorTorque = 8,
            /// <summary>The left torque</summary>
            LeftTorque = 9,
            /// <summary>The right torque</summary>
            RightTorque = 10,
            /// <summary>The torque y axis</summary>
            TorqueYAxis = 11,
            /// <summary>The torque outboardness</summary>
            TorqueOutboardness = 12,
            /// <summary>The whole sensor force</summary>
            WholeSensorForce = 16,
            /// <summary>The left force</summary>
            LeftForce = 17,
            /// <summary>The right force</summary>
            RightForce = 18,
            /// <summary>The crank angle</summary>
            CrankAngle = 20,
            /// <summary>The left crank angle</summary>
            LeftCrankAngle = 21,
            /// <summary>The right crank angle</summary>
            RightCrankAngle = 22,
            /// <summary>The zero offset</summary>
            ZeroOffset = 24,
            /// <summary>The temperature</summary>
            Temperature = 25,
            /// <summary>The voltage</summary>
            Voltage = 26,
            /// <summary>The left force forward</summary>
            LeftForceForward = 32,
            /// <summary>The right force forward</summary>
            RightForceForward = 33,
            /// <summary>The left force downward</summary>
            LeftForceDownward = 34,
            /// <summary>The right force downward</summary>
            RightForceDownward = 35,
            /// <summary>The left pedal angle</summary>
            LeftPedalAngle = 40,
            /// <summary>The right pedal angle</summary>
            RightPedalAngle = 41
        }

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Gets the number of measurement types.</summary>
        public int NumberOfMeasurementTypes { get; }
        /// <summary>Gets the type of this measurement.</summary>
        public DataType MeasurementType { get; }
        /// <summary>Gets the timestamp of the measurement.</summary>
        public double Timestamp { get; private set; }
        /// <summary>Gets the measurement value. The scaling factor is applied.</summary>
        public double Measurement { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasurementOutputData"/> class.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        internal MeasurementOutputData(byte[] dataPage)
        {
            NumberOfMeasurementTypes = dataPage[1];
            MeasurementType = (DataType)dataPage[2];
        }

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        internal void Parse(byte[] dataPage)
        {
            Timestamp = BitConverter.ToUInt16(dataPage, 4) / 2048.0;
            Measurement = BitConverter.ToInt16(dataPage, 6) * Math.Pow(2, (sbyte)dataPage[3]);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Timestamp)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Measurement)));
        }
    }
}
