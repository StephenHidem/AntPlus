using AntPlus;
using System;
using System.Linq;

namespace DeviceProfiles
{
    /// <summary>
    /// The HeartRate class provides full support for ANT+ heart rate monitors. This profile is specified in the document
    /// ANT+ Managed Network Document – ANT+ Heart Rate Device Profile, Rev 2.1, © 2006-2016 Dynastream Innovations Inc. All Rights Reserved.
    /// 
    /// © 2022 Stephen Hidem.
    /// </summary>
    /// <remarks>
    /// This is purely an event driven class. Any pages received will generate a <see cref="HeartRateChanged"/> and the appropriate page event
    /// once the page toggle has been observed. Consumers shoud attach event handlers to the pages of interest.
    /// 
    /// Due to the ubiquity of heart rate monitors and manufacturers, several problems present themselves with regard to this specification.
    /// This primarily relates to group workout environments (clubs). These issues do not exist for individual workout environments (homes).
    /// 
    /// Because of the limitations posed by the channel ID it is a distinct possibility that different HRM's will have the same channel ID.
    /// 1. There is no guarantee the channel ID is unique to a specific device.
    /// 2. There is no guarantee the background page sent is unique to a specific device.
    /// 3. RequestCapabilities will be broadcast to all devices with the same channel ID. You may receive multiple contradictory replies.
    /// 4. SetSportMode will be broadcast to all devices with the same channel ID. This is not an issue if the invocation is local to the group involved.
    /// 
    /// Keep these caveats in mind when designing your application. Applications designed for group environments may choose to only attach
    /// to the manufacturer event and ignore other background pages.
    /// </remarks>
    /// <seealso cref="AntPlus.AntDevice" />
    public class HeartRate : AntDevice
    {
        /// <summary>
        /// The heart rate device class ID.
        /// </summary>
        public const byte DeviceClass = 120;

        /// <summary>
        /// Heart rate device data pages.
        /// </summary>
        public enum DataPage
        {
            /// <summary>Default or unknown data page.</summary>
            Default,
            /// <summary>Cumulative operating time</summary>
            CumulativeOperatingTime,
            /// <summary>Manufacturer information</summary>
            ManufacturerInfo,
            /// <summary>Product information</summary>
            ProductInfo,
            /// <summary>Previous heart beat</summary>
            PreviousHeartBeat,
            /// <summary>Swim interval</summary>
            SwimInterval,
            /// <summary>Capabilities</summary>
            Capabilities,
            /// <summary>Battery status</summary>
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
            /// <summary>All</summary>
            All = 7
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
        private int accumHeartBeatCount;
        private ushort lastBeatEventTime;
        private int accumHeartBeatEventTime;
        private bool pageToggle = false;
        private int observedToggle;

        /// <summary>
        /// Heart rate data common to all data pages.
        /// </summary>
        public struct CommonHeartRateData
        {
            /// <summary>Accumulated heart beat event time in milliseconds.</summary>
            public int AccumulatedHeartBeatEventTime;
            /// <summary>Accumulated heart beat count.</summary>
            public int AccumulatedHeartBeatCount;
            /// <summary>Computed heart rate as determined by the sensor in beats per minute.</summary>
            public byte ComputedHeartRate;

            internal CommonHeartRateData(int accumEventTime, int accumBeatCount, byte heartRate)
            {
                AccumulatedHeartBeatEventTime = (int)((long)accumEventTime * 1000 / 1024);
                AccumulatedHeartBeatCount = accumBeatCount;
                ComputedHeartRate = heartRate;
            }
        }

        /// <summary>
        /// Manufacturer supplied info. It may be sent as a main page or background page. The heart beat event time and count are
        /// provided as-is from this data page. It is up to the application if these values should be accumulated or utilized.
        /// </summary>
        public struct ManufacturerInfoPage
        {
            /// <summary>The manufacturing identifier LSB</summary>
            public byte ManufacturingIdLsb;
            /// <summary>The serial number</summary>
            public uint SerialNumber;
            /// <summary>Heart beat event time in 1/1024 of a second. Rollover at 65536 (64 seconds).</summary>
            public ushort HeartBeatEventTime;
            /// <summary>Heart beat count. Rollover at 256.</summary>
            public byte HeartBeatCount;
            /// <summary>Computed heart rate as determined by the sensor in beats per minute.</summary>
            public byte ComputedHeartRate;

            internal ManufacturerInfoPage(byte[] page, uint deviceNumber)
            {
                ManufacturingIdLsb = page[1];
                SerialNumber = (uint)((BitConverter.ToUInt16(page, 2) << 16) + (deviceNumber & 0x0000FFFF));
                HeartBeatEventTime = BitConverter.ToUInt16(page, 4);
                HeartBeatCount = page[6];
                ComputedHeartRate = page[7];
            }
        }

        /// <summary>
        /// Product information. Sent as a background page. All fields are manufacturer specific.
        /// </summary>
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
        /// This structure provides the RR interval and manufacturer specific data. It is typically sent as a main page.
        /// </summary>
        public struct PreviousHeartBeatPage
        {
            /// <summary>Manufacturer specific data. Set to 0xFF if not used.</summary>
            public byte ManufacturerSpecific;
            /// <summary>RR interval in milliseconds.</summary>
            public int RRInterval;

            internal PreviousHeartBeatPage(byte[] page)
            {
                ManufacturerSpecific = page[1];
                RRInterval = CalculateRRInverval(BitConverter.ToUInt16(page, 2), BitConverter.ToUInt16(page, 4));
            }
        }

        /// <summary>
        /// Swim interval data
        /// </summary>
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

        /// <summary>
        /// Heart rate device capabilities
        /// </summary>
        public struct CapabilitiesPage
        {
            /// <summary>Enabled features.</summary>
            public Features Enabled;
            /// <summary>Supported features.</summary>
            public Features Supported;
            /// <summary>Manufacturer specific features.</summary>
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
            /// <summary>The manufacturer specific page number.</summary>
            public byte Page;
            /// <summary>The manufacturer specific data.</summary>
            public byte[] Data;

            internal ManufacturerSpecificPage(byte[] page)
            {
                Page = page[0];
                Data = page.Skip(1).Take(3).ToArray();
            }
        }

        /// <summary>
        /// Occurs when heart rate changed. This data is common to all pages transmitted.
        /// </summary>
        public event EventHandler<CommonHeartRateData> HeartRateChanged;
        /// <summary>
        /// Occurs when cumulative operating time page changed.
        /// </summary>
        public event EventHandler<TimeSpan> CumulativeOperatingTimePageChanged;
        /// <summary>
        /// Occurs when manufacturer information page changed.
        /// </summary>
        public event EventHandler<ManufacturerInfoPage> ManufacturerInfoPageChanged;
        /// <summary>
        /// Occurs when product information page changed.
        /// </summary>
        public event EventHandler<ProductInfoPage> ProductInfoPageChanged;
        /// <summary>
        /// Occurs when previous heart beat page changed.
        /// </summary>
        public event EventHandler<PreviousHeartBeatPage> PreviousHeartBeatPageChanged;
        /// <summary>
        /// Occurs when swim interval page changed.
        /// </summary>
        public event EventHandler<SwimIntervalPage> SwimIntervalPageChanged;
        /// <summary>
        /// Occurs when capabilities page changed.
        /// </summary>
        public event EventHandler<CapabilitiesPage> CapabilitiesPageChanged;
        /// <summary>
        /// Occurs when battery status page changed.
        /// </summary>
        public event EventHandler<BatteryStatusPage> BatteryStatusPageChanged;
        /// <summary>
        /// Occurs when manufacturer specific page changed.
        /// </summary>
        public event EventHandler<ManufacturerSpecificPage> ManufacturerSpecificPageChanged;


        /// <summary>
        /// Initializes a new instance of the <see cref="HeartRate"/> class.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        public HeartRate(ChannelId channelId) : base(channelId)
        {
        }

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public override void Parse(byte[] dataPage)
        {
            // ignore duplicate/unchanged data pages
            if (lastDataPage.SequenceEqual(dataPage))
            {
                return;
            }
            lastDataPage = dataPage;

            // this data is present in all data pages
            accumHeartBeatEventTime += CalculateDelta(BitConverter.ToUInt16(dataPage, 4), ref lastBeatEventTime);
            accumHeartBeatCount += CalculateDelta(dataPage[6], ref lastBeatCount);
            HeartRateChanged?.Invoke(this, new CommonHeartRateData(accumHeartBeatEventTime, accumHeartBeatCount, dataPage[7]));

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
                    ManufacturerInfoPageChanged?.Invoke(this, new ManufacturerInfoPage(dataPage, ChannelId.DeviceNumber));
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

        /// <summary>Requests the capabilities.</summary>
        /// <param name="channelNumber">Channel to send message. Default is channel 0.</param>
        /// <param name="numberOfTimesToTransmit">The number of times to transmit. Default is 4 times.</param>
        public void RequestCapabilities(byte channelNumber = 0, byte numberOfTimesToTransmit = 4)
        {
            RequestDataPage((byte)DataPage.Capabilities, channelNumber, numberOfTimesToTransmit);
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
        /// <returns>RR interval in milliseconds.</returns>
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
