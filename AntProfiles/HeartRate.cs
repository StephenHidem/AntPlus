using AntPlus;
using System;

// TODO: PAGE DATA TOGGLE
// TODO: MANUFACTURING SPECIFIC PAGES
// TODO: MANUFACTURING SPECIFIC CAPABILITIES
namespace AntPlusDeviceProfiles
{
    public class HeartRate : AntDevice
    {
        public const byte DeviceClass = 120;

        public enum DataPage
        {
            Default,
            CumulativeOperatingTime,
            ManufacturerInfo,
            ProductInfo,
            PreviousHeartBeat,
            SwimInterval,
            Capabilities,
            BatteryStatus
        }

        [Flags]
        public enum Features
        {
            None = 0,
            Running = 1,
            Cycling = 2,
            Swimming = 4,
        }

        public enum SportMode
        {
            None,
            Running,
            Cycling,
            Swimming = 5,
        }

        private byte lastBeatCount;
        private int previousAccumulatedBeatCount;
        private ushort lastBeatEventTime;
        private ushort lastPreviousBeatEventTime;

        // common to all heart rate messages
        public int AccumulatedHeartBeatEventTime { get; private set; }
        public int AccumulatedHeartBeatCount { get; private set; }
        public byte ComputedHeartRate { get; private set; }

        public TimeSpan CumulativeOperatingTime { get; private set; }

        // manufacturing info
        public byte ManufacturingIdLsb { get; private set; }
        public uint SerialNumber { get; private set; }

        // product info
        public byte HardwareVersion { get; private set; }
        public byte SoftwareVersion { get; private set; }
        public byte ModelNumber { get; private set; }

        // previous heart beat
        public byte ManufacturerSpecific { get; private set; }
        public int AccumulatedPreviousHeartBeatEventTime { get; private set; }
        public int RRInterval { get; private set; }

        // swim interval
        public byte IntervalAverageHeartRate { get; private set; }
        public byte IntervalMaximumHeartRate { get; private set; }
        public byte SessionAverageHeartRate { get; private set; }

        // capabilities
        public Features Enabled { get; private set; }
        public Features Supported { get; private set; }
        public int ManufacturerSpecificFeatures { get; private set; }

        // battery status
        public byte BatteryLevel { get; private set; }
        public double BatteryVoltage { get; private set; }
        public BatteryStatus BatteryStatus { get; private set; }

        public HeartRate(byte[] payload, uint channelId) : base(payload, channelId)
        {
        }

        public void Parse(byte[] payload)
        {
            // check for valid payload
            if (payload == null || payload.Length != 8)
            {
                return;
            }

            // this data is present in all pages
            AccumulatedHeartBeatEventTime = UpdateAccumulatedValue(BitConverter.ToUInt16(payload, 4), ref lastBeatEventTime, AccumulatedHeartBeatEventTime);
            AccumulatedHeartBeatCount = UpdateAccumulatedValue(payload[6], ref lastBeatCount, AccumulatedHeartBeatCount);
            ComputedHeartRate = payload[7];
            previousAccumulatedBeatCount = AccumulatedHeartBeatCount;

            switch ((DataPage)(payload[0] & 0x7F))
            {
                case DataPage.CumulativeOperatingTime:
                    CumulativeOperatingTime = TimeSpan.FromSeconds((BitConverter.ToUInt32(payload, 1) & 0x00FFFFFF) * 2.0);
                    break;
                case DataPage.ManufacturerInfo:
                    ManufacturingIdLsb = payload[1];
                    SerialNumber = (uint)((BitConverter.ToUInt16(payload, 2) << 16) + (DeviceNumber & 0x0000FFFF));
                    break;
                case DataPage.ProductInfo:
                    HardwareVersion = payload[1];
                    SoftwareVersion = payload[2];
                    ModelNumber = payload[3];
                    break;
                case DataPage.PreviousHeartBeat:
                    ManufacturerSpecific = payload[1];
                    AccumulatedPreviousHeartBeatEventTime = UpdateAccumulatedValue(BitConverter.ToUInt16(payload, 2), ref lastPreviousBeatEventTime, AccumulatedPreviousHeartBeatEventTime);
                    RRInterval = CalculateRRInverval(AccumulatedPreviousHeartBeatEventTime, AccumulatedHeartBeatEventTime);
                    break;
                case DataPage.SwimInterval:
                    IntervalAverageHeartRate = payload[1];
                    IntervalMaximumHeartRate = payload[2];
                    SessionAverageHeartRate = payload[3];
                    break;
                case DataPage.Capabilities:
                    Enabled = (Features)(payload[2] & 0x07);
                    Supported = (Features)(payload[3] & 0x07);
                    ManufacturerSpecificFeatures = payload[3] >> 6;
                    break;
                case DataPage.BatteryStatus:
                    BatteryLevel = payload[1];
                    BatteryVoltage = (payload[3] & 0x0F) + (payload[2] / 256.0);
                    BatteryStatus = (BatteryStatus)((payload[3] & 0x70) >> 4);
                    break;
                default:
                    // range check manufacturer specific pages
                    if (payload[0] >= 112 && payload[0] < 128)
                    {
                        // TODO: let manufacturer parse
                    }
                    break;
            }
            isFirstDataMessage = false;
        }

        private int CalculateRRInverval(int previousHeartBeatEventTime, int heartBeatEventTime)
        {
            return (heartBeatEventTime - previousHeartBeatEventTime) * 1000 / 1024;
        }
    }
}
