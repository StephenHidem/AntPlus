using AntPlus;
using System;
using System.Linq;

namespace AntPlusDeviceProfiles
{
    /// <summary>
    /// The HeartRate class provides full support for ANT+ heart rate monitors. This profile is specified in the document
    /// ANT+ Managed Network Document – ANT+ Heart Rate Device Profile, Rev 2.1.
    /// </summary>
    /// <seealso cref="AntPlus.AntDevice" />
    public class HeartRate : AntDevice
    {
        /// <summary>
        /// The heart rate device class ID.
        /// </summary>
        public static byte DeviceClass = 120;

        /// <summary>
        ///   <br />
        /// </summary>
        public enum DataPage
        {
            /// <summary>The default or unknown data page.</summary>
            Default,
            /// <summary>The cumulative operating time</summary>
            CumulativeOperatingTime,
            /// <summary>The manufacturer information</summary>
            ManufacturerInfo,
            /// <summary>The product information</summary>
            ProductInfo,
            /// <summary>The previous heart beat</summary>
            PreviousHeartBeat,
            /// <summary>The swim interval</summary>
            SwimInterval,
            /// <summary>The capabilities</summary>
            Capabilities,
            /// <summary>The battery status</summary>
            BatteryStatus
        }

        /// <summary>
        /// Supported feature flags from the capabilities page.
        /// </summary>
        [Flags]
        public enum Features
        {
            /// <summary>No capabilities</summary>
            None = 0,
            /// <summary>Running</summary>
            Running = 1,
            /// <summary>Cycling</summary>
            Cycling = 2,
            /// <summary>Swimming</summary>
            Swimming = 4,
        }

        /// <summary>
        /// Use this enumeration to set the desired sport mode.
        /// </summary>
        public enum SportMode
        {
            /// <summary>Clear sport mode setting</summary>
            None,
            /// <summary>Running</summary>
            Running,
            /// <summary>Cycling</summary>
            Cycling,
            /// <summary>Swimming</summary>
            Swimming = 5,
        }

        private byte[] lastDataPage = new byte[8];
        private byte lastBeatCount;
        private ushort lastBeatEventTime;
        private bool pageToggle = false;
        private int observedToggle;

        public struct ManufacturerInfoPage
        {
            public byte ManufacturingIdLsb;
            public uint SerialNumber;

            internal ManufacturerInfoPage(byte[] page, uint deviceNumber)
            {
                ManufacturingIdLsb = page[1];
                SerialNumber = (uint)((BitConverter.ToUInt16(page, 2) << 16) + (deviceNumber & 0x0000FFFF));
            }
        }

        public struct ProductInfoPage
        {
            /// <summary>Gets the hardware version.</summary>
            public byte HardwareVersion;
            /// <summary>Gets the software version.</summary>
            public byte SoftwareVersion;
            /// <summary>Gets the model number.</summary>
            public byte ModelNumber;

            internal ProductInfoPage(byte[] page)
            {
                HardwareVersion = page[1];
                SoftwareVersion = page[2];
                ModelNumber = page[3];
            }
        }

        /// <summary>
        /// This structure provides the RR interval and manufacturer specific data. It is typically sent a main page.
        /// </summary>
        public struct PreviousHeartBeatPage
        {
            /// <summary>
            /// The manufacturer specific data for the previous heart beat page.
            /// </summary>
            public byte ManufacturerSpecific;
            /// <summary>Gets the RR interval.</summary>
            public int RRInterval;

            /// <summary>
            /// Initializes a new instance of the <see cref="PreviousHeartBeatPage"/> struct.
            /// </summary>
            /// <param name="page">The page.</param>
            internal PreviousHeartBeatPage(byte[] page)
            {
                ManufacturerSpecific = page[1];
                RRInterval = CalculateRRInverval(BitConverter.ToUInt16(page, 2), BitConverter.ToUInt16(page, 4));
            }
        }

        public struct SwimIntervalPage
        {
            /// <summary>Swim interval average heart rate.</summary>
            public byte IntervalAverageHeartRate;
            /// <summary>Swim interval maximum heart rate.</summary>
            public byte IntervalMaximumHeartRate;
            /// <summary>Swim session average heart rate.</summary>
            public byte SessionAverageHeartRate;

            internal SwimIntervalPage(byte[] page)
            {
                IntervalAverageHeartRate = page[1];
                IntervalMaximumHeartRate = page[2];
                SessionAverageHeartRate = page[3];
            }
        }

        public struct CapabilitiesPage
        {
            /// <summary>
            /// Enabled features.
            /// </summary>
            public Features Enabled;
            /// <summary>
            /// Supported features.
            /// </summary>
            public Features Supported;
            /// <summary>
            /// Manufacturer specific features.
            /// </summary>
            public int ManufacturerSpecificFeatures;

            internal CapabilitiesPage(byte[] page)
            {
                Supported = (Features)(page[2] & 0x07);
                Enabled = (Features)(page[3] & 0x07);
                ManufacturerSpecificFeatures = page[3] >> 6;
            }
        }

        /// <summary>
        /// The battery status page. Sent as a background page.
        /// </summary>
        public struct BatteryStatusPage
        {
            /// <summary>Battery level.</summary>
            public byte BatteryLevel { get; private set; }
            /// <summary>Battery voltage.</summary>
            public double BatteryVoltage { get; private set; }
            /// <summary>Battery status.</summary>
            public BatteryStatus BatteryStatus { get; private set; }

            internal BatteryStatusPage(byte[] page)
            {
                BatteryLevel = page[1];
                BatteryVoltage = (page[3] & 0x0F) + (page[2] / 256.0);
                BatteryStatus = (BatteryStatus)((page[3] & 0x70) >> 4);
            }
        }

        /// <summary>
        /// This structure provides manufacturer specific data. Sent as a background page.
        /// </summary>
        public struct ManufacturerSpecificPage
        {
            /// <summary>
            /// The manufacturer specific page number.
            /// </summary>
            public byte Page;
            /// <summary>
            /// The manufacturer specific data.
            /// </summary>
            public byte[] Data;

            /// <summary>
            /// Initializes a new instance of the <see cref="ManufacturerSpecificPage"/> struct.
            /// </summary>
            /// <param name="page">The page.</param>
            internal ManufacturerSpecificPage(byte[] page)
            {
                Page = page[0];
                Data = page.Skip(1).Take(3).ToArray();
            }
        }

        public event EventHandler<DataPage> DataPageChanged;
        /// <summary>
        /// Occurs when [cumulative operating time page changed].
        /// </summary>
        public event EventHandler<TimeSpan> CumulativeOperatingTimePageChanged;
        public event EventHandler<ManufacturerInfoPage> ManufacturerInfoPageChanged;
        public event EventHandler<ProductInfoPage> ProductInfoPageChanged;
        /// <summary>
        /// Occurs when [previous heart beat page changed].
        /// </summary>
        public event EventHandler<PreviousHeartBeatPage> PreviousHeartBeatPageChanged;
        public event EventHandler<SwimIntervalPage> SwimIntervalPageChanged;
        public event EventHandler<CapabilitiesPage> CapabilitiesPageChanged;
        public event EventHandler<BatteryStatusPage> BatteryStatusPageChanged;
        /// <summary>
        /// Occurs when [manufacturer specific page changed].
        /// </summary>
        public event EventHandler<ManufacturerSpecificPage> ManufacturerSpecificPageChanged;

        // common to all heart rate messages
        /// <summary>Gets the accumulated heart beat event time.</summary>
        /// <value>The accumulated heart beat event time.</value>
        public int AccumulatedHeartBeatEventTime { get; private set; }
        /// <summary>Gets the accumulated heart beat count.</summary>
        /// <value>The accumulated heart beat count.</value>
        public int AccumulatedHeartBeatCount { get; private set; }
        /// <summary>Gets the computed heart rate as determined by the sensor.</summary>
        /// <value>The computed heart rate.</value>
        public byte ComputedHeartRate { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="HeartRate"/> class.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="channelId">The channel identifier.</param>
        public HeartRate(byte[] payload, uint channelId) : base(payload, channelId)
        {
        }

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public override void Parse(byte[] dataPage)
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
            AccumulatedHeartBeatEventTime += CalculateDelta(BitConverter.ToUInt16(dataPage, 4), ref lastBeatEventTime);
            AccumulatedHeartBeatCount += CalculateDelta(dataPage[6], ref lastBeatCount);
            ComputedHeartRate = dataPage[7];

            // handle data page toggle
            if (isFirstDataMessage)
            {
                observedToggle = dataPage[0] & 0x80;
                isFirstDataMessage = false;
                return;
            }
            else
            {
                if (!pageToggle)
                {
                    if ((dataPage[0] & 0x80) != observedToggle)
                    {
                        pageToggle = true;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            switch ((DataPage)(dataPage[0] & 0x7F))
            {
                case DataPage.CumulativeOperatingTime:
                    CumulativeOperatingTimePageChanged?.Invoke(this, TimeSpan.FromSeconds((BitConverter.ToUInt32(dataPage, 1) & 0x00FFFFFF) * 2.0));
                    break;
                case DataPage.ManufacturerInfo:
                    ManufacturerInfoPageChanged?.Invoke(this, new ManufacturerInfoPage(dataPage, DeviceNumber));
                    break;
                case DataPage.ProductInfo:
                    ProductInfoPageChanged?.Invoke(this, new ProductInfoPage(dataPage));
                    break;
                case DataPage.PreviousHeartBeat:
                    PreviousHeartBeatPageChanged?.Invoke(this, new PreviousHeartBeatPage(dataPage));
                    break;
                case DataPage.SwimInterval:
                    SwimIntervalPageChanged?.Invoke(this, new SwimIntervalPage(dataPage));
                    break;
                case DataPage.Capabilities:
                    CapabilitiesPageChanged?.Invoke(this, new CapabilitiesPage(dataPage));
                    break;
                case DataPage.BatteryStatus:
                    BatteryStatusPageChanged?.Invoke(this, new BatteryStatusPage(dataPage));
                    break;
                default:
                    // range check manufacturer specific pages
                    if (dataPage[0] >= 112 && dataPage[0] < 128)
                    {
                        // let application parse
                        ManufacturerSpecificPageChanged?.Invoke(this, new ManufacturerSpecificPage(dataPage));
                    }
                    break;
            }
        }

        /// <summary>
        /// Requests the capabilities.
        /// </summary>
        /// <param name="numberOfTimesToTransmit">The number of times to transmit.</param>
        public void RequestCapabilities(byte numberOfTimesToTransmit = 4)
        {
            RequestDataPage((byte)DataPage.Capabilities, numberOfTimesToTransmit);
        }

        /// <summary>
        /// Sets the sport mode.
        /// </summary>
        /// <param name="sportMode">The sport mode.</param>
        public void SetSportMode(SportMode sportMode)
        {
            byte[] msg = new byte[] { (byte)CommonDataPageType.ModeSettingsPage, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, (byte)sportMode };
            SendExtendedAcknowledgedMessage(0, msg);
        }

        /// <summary>
        /// Calculates the RR inverval.
        /// </summary>
        /// <param name="previousHeartBeatEventTime">The previous heart beat event time.</param>
        /// <param name="heartBeatEventTime">The heart beat event time.</param>
        /// <returns></returns>
        private static int CalculateRRInverval(ushort previousHeartBeatEventTime, ushort heartBeatEventTime)
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
