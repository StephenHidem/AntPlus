using AntPlus;
using System;
using System.Linq;

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

        private byte[] lastDataPage = new byte[8];
        private byte lastBeatCount;
        private ushort lastBeatEventTime;

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

        public void Parse(byte[] dataPage)
        {
            // ignore invalid data pages
            if (dataPage == null || dataPage.Length != 8)
            {
                return;
            }

            // ignore duplicate/unchanged data pages
            if (lastDataPage.SequenceEqual(dataPage))
            {
                return;
            }
            lastDataPage = dataPage;

            // this data is present in all data pages
            AccumulatedHeartBeatEventTime = UpdateAccumulatedValue(BitConverter.ToUInt16(dataPage, 4), ref lastBeatEventTime, AccumulatedHeartBeatEventTime);
            AccumulatedHeartBeatCount = UpdateAccumulatedValue(dataPage[6], ref lastBeatCount, AccumulatedHeartBeatCount);
            ComputedHeartRate = dataPage[7];

            // TODO: CONTINUE IF DATA PAGE TOGGLE HAS BEEN OBSERVED
            switch ((DataPage)(dataPage[0] & 0x7F))
            {
                case DataPage.CumulativeOperatingTime:
                    CumulativeOperatingTime = TimeSpan.FromSeconds((BitConverter.ToUInt32(dataPage, 1) & 0x00FFFFFF) * 2.0);
                    break;
                case DataPage.ManufacturerInfo:
                    ManufacturingIdLsb = dataPage[1];
                    SerialNumber = (uint)((BitConverter.ToUInt16(dataPage, 2) << 16) + (DeviceNumber & 0x0000FFFF));
                    break;
                case DataPage.ProductInfo:
                    HardwareVersion = dataPage[1];
                    SoftwareVersion = dataPage[2];
                    ModelNumber = dataPage[3];
                    break;
                case DataPage.PreviousHeartBeat:
                    ManufacturerSpecific = dataPage[1];
                    RRInterval = CalculateRRInverval(BitConverter.ToUInt16(dataPage, 2), BitConverter.ToUInt16(dataPage, 4));
                    break;
                case DataPage.SwimInterval:
                    IntervalAverageHeartRate = dataPage[1];
                    IntervalMaximumHeartRate = dataPage[2];
                    SessionAverageHeartRate = dataPage[3];
                    break;
                case DataPage.Capabilities:
                    Enabled = (Features)(dataPage[2] & 0x07);
                    Supported = (Features)(dataPage[3] & 0x07);
                    ManufacturerSpecificFeatures = dataPage[3] >> 6;
                    break;
                case DataPage.BatteryStatus:
                    BatteryLevel = dataPage[1];
                    BatteryVoltage = (dataPage[3] & 0x0F) + (dataPage[2] / 256.0);
                    BatteryStatus = (BatteryStatus)((dataPage[3] & 0x70) >> 4);
                    break;
                default:
                    // range check manufacturer specific pages
                    if (dataPage[0] >= 112 && dataPage[0] < 128)
                    {
                        // TODO: let manufacturer parse
                    }
                    break;
            }
            isFirstDataMessage = false;
        }

        /// <summary>
        /// Requests the capabilities.
        /// </summary>
        /// <param name="numberOfTimesToTransmit">The number of times to transmit.</param>
        public void RequestCapabilities(byte numberOfTimesToTransmit = 4)
        {
            RequestDataPage((byte)DataPage.Capabilities, numberOfTimesToTransmit);
        }

        // TODO: IMPLEMENT SET SPORT MODE
        public void SetSportMode(SportMode sportMode)
        {
            byte[] msg = new byte[] { (byte)CommonDataPageType.ModeSettingsPage, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, (byte)sportMode };
            SendExtendedAcknowledgedMessage(0, msg);
        }

        private int CalculateRRInverval(ushort previousHeartBeatEventTime, ushort heartBeatEventTime)
        {
            // calculate delta event time
            var deltaEventTime = heartBeatEventTime - previousHeartBeatEventTime;

            // handle rollover
            if (deltaEventTime < 0)
                deltaEventTime += 0x10000;

            // convert to milliseconds
            return deltaEventTime * 1000 / 1024;
        }
    }
}
