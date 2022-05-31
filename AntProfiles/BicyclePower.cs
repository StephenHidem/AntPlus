using AntPlus;
using System;

namespace AntPlusDeviceProfiles
{
    public class BicyclePower : AntDevice
    {
        public const byte DeviceClass = 11;

        public enum DataPage
        {
            Unknown,
            Calibration,
            GetSetParameters,
            MeasurementOutput,
            PowerOnly = 0x10,
            WheelTorque,
            CrankTorque,
            TorqueEffectivenessAndPedalSmoothness,
            TorqueBarycenter,
            CrankTorqueFrequency = 0x20,
            RightForceAngle = 0xE0,
            LeftForceAngle,
            PedalPosition
        }

        private byte lastEventCount;
        private int previousAccumulatedEventCount;

        private ushort lastPower;
        private int previousAccumulatedPower;

        private byte lastTicks;
        private ushort lastPeriod;
        private int previousAccumulatedPeriod;
        private ushort lastTorque;
        private int previousAccumulatedTorque;

        public int AccumulatedEventCount { get; private set; }
        public double AveragePower { get; private set; }

        // Standard Power-Only Main Data Page (0x10)
        public byte PedalPower { get; private set; }
        public byte InstantaneousCadence { get; private set; }
        public int AccumulatedPower { get; private set; }
        public ushort InstantaneousPower { get; private set; }

        // Standard Wheel Torque Main Data Page (0x11) and Crank Torque Main Data Page (0x12)
        public int AccumulatedTicks { get; private set; }
        public int AccumulatedPeriod { get; private set; }
        public int AccumulatedTorque { get; private set; }
        public double AverageAngularVelocity { get; private set; }
        public double AverageTorque { get; private set; }


        public BicyclePower(byte[] payload, uint channelId) : base(payload, channelId)
        {
        }

        public void Parse(byte[] payload)
        {
            // check for valid payload
            if (payload == null || payload.Length != 8)
            {
                return;
            }

            switch ((DataPage)payload[0])
            {
                case DataPage.Unknown:
                    break;
                case DataPage.Calibration:
                    break;
                case DataPage.GetSetParameters:
                    break;
                case DataPage.MeasurementOutput:
                    break;
                case DataPage.PowerOnly:
                    AccumulatedEventCount += CalculateDelta(payload[1], ref lastEventCount);
                    PedalPower = payload[2];
                    InstantaneousCadence = payload[3];
                    AccumulatedPower += CalculateDelta(BitConverter.ToUInt16(payload, 4), ref lastPower);
                    InstantaneousPower = BitConverter.ToUInt16(payload, 6);
                    AveragePower = (AccumulatedPower - previousAccumulatedPower) / (AccumulatedEventCount - previousAccumulatedEventCount);
                    previousAccumulatedEventCount = AccumulatedEventCount;
                    previousAccumulatedPower = AccumulatedPower;
                    break;
                case DataPage.WheelTorque:
                    AccumulatedEventCount += CalculateDelta(payload[1], ref lastEventCount);
                    AccumulatedTicks += CalculateDelta(payload[2], ref lastTicks);
                    InstantaneousCadence = payload[3];
                    AccumulatedPeriod += CalculateDelta(BitConverter.ToUInt16(payload, 4), ref lastPeriod);
                    AccumulatedTorque += CalculateDelta(BitConverter.ToUInt16(payload, 6), ref lastTorque);
                    AverageAngularVelocity = ComputeAveAngularVelocity();
                    AverageTorque = ComputeAveTorque();
                    AveragePower = AverageTorque * AverageAngularVelocity;
                    previousAccumulatedEventCount = AccumulatedEventCount;
                    previousAccumulatedPeriod = AccumulatedPeriod;
                    previousAccumulatedTorque = AccumulatedTorque;
                    break;
                case DataPage.CrankTorque:
                    AccumulatedEventCount += CalculateDelta(payload[1], ref lastEventCount);
                    AccumulatedTicks += CalculateDelta(payload[2], ref lastTicks);
                    InstantaneousCadence = payload[3];
                    AccumulatedPeriod += CalculateDelta(BitConverter.ToUInt16(payload, 4), ref lastPeriod);
                    AccumulatedTorque += CalculateDelta(BitConverter.ToUInt16(payload, 6), ref lastTorque);
                    AverageAngularVelocity = ComputeAveAngularVelocity();
                    AverageTorque = ComputeAveTorque();
                    AveragePower = AverageTorque * AverageAngularVelocity;
                    previousAccumulatedEventCount = AccumulatedEventCount;
                    previousAccumulatedPeriod = AccumulatedPeriod;
                    previousAccumulatedTorque = AccumulatedTorque;
                    break;
                case DataPage.TorqueEffectivenessAndPedalSmoothness:
                    break;
                case DataPage.CrankTorqueFrequency:
                    break;
                case DataPage.RightForceAngle:
                    break;
                case DataPage.LeftForceAngle:
                    break;
                case DataPage.PedalPosition:
                    break;
                default:
                    break;
            }
        }

        private double ComputeAveAngularVelocity()
        {
            return 2 * Math.PI * (AccumulatedEventCount - previousAccumulatedEventCount) / ((AccumulatedPeriod - previousAccumulatedPeriod) / 2048);
        }

        private double ComputeAveTorque()
        {
            return (AccumulatedTorque - previousAccumulatedTorque) / (32 * (AccumulatedEventCount - previousAccumulatedEventCount));
        }
    }
}
