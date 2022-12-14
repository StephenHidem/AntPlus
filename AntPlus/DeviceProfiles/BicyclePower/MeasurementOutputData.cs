using System;

namespace DeviceProfiles.BicyclePower
{
    public readonly struct MeasurementOutputData
    {
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

        public int NumberOfMeasurementTypes { get; }
        public DataType MeasurementType { get; }
        public double Timestamp { get; }
        public double Measurement { get; }

        internal MeasurementOutputData(byte[] page)
        {
            NumberOfMeasurementTypes = page[1];
            MeasurementType = (DataType)page[2];
            Timestamp = BitConverter.ToUInt16(page, 4) / 2048.0;
            Measurement = BitConverter.ToUInt16(page, 6) * Math.Pow(2, (sbyte)page[3]);
        }
    }
}
