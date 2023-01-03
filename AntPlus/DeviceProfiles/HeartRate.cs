using AntRadioInterface;
using System;
using System.ComponentModel;
using System.Linq;

namespace AntPlus.DeviceProfiles.HeartRate
{
    /// <summary>
    /// The HeartRate class provides full support for ANT+ heart rate monitors. This profile is specified in the document
    /// ANT+ Managed Network Document – ANT+ Heart Rate Device Profile, Rev 2.5, © 2006-2022 Garmin Canada Inc. All Rights Reserved.
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
    public class HeartRate : AntDevice, INotifyPropertyChanged
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
            BatteryStatus,
            /// <summary>
            /// Device Information
            /// </summary>
            DeviceInformation = 9,
            /// <summary>
            /// Heart feature. Sent from display.
            /// </summary>
            HRFeature = 32
        }

        /// <summary>
        /// Supported feature flags from the capabilities page.
        /// </summary>
        [Flags]
        public enum Features
        {
            /// <summary>Generic capabilities</summary>
            Generic = 0,
            /// <summary>Running</summary>
            Running = 1,
            /// <summary>Cycling</summary>
            Cycling = 2,
            /// <summary>Swimming</summary>
            Swimming = 4,
            /// <summary>Gym mode</summary>
            GymMode = 8,
            /// <summary>Manufacture defined feature</summary>
            ManufacturerFeature1 = 0x40,
            /// <summary>Manufacture defined feature</summary>
            ManufacturerFeature2 = 0x80,
            All = 0xCF
        }

        public enum HeartbeatEventType
        {
            MeasuredTimestamp,
            ComputedTimestamp
        }

        private bool isFirstDataMessage = true;     // used for accumulated values
        private byte prevBeatCount;
        private ushort prevBeatEventTime;
        private int accumHeartBeatEventTime;
        private bool pageToggle = false;
        private int observedToggle;
        private int rrInterval;

        /// <summary>
        /// Heart rate data common to all data pages.
        /// </summary>
        public readonly struct CommonHeartRateData
        {
            /// <summary>Accumulated heart beat event time in milliseconds.</summary>
            public int AccumulatedHeartBeatEventTime { get; }
            /// <summary>Computed heart rate as determined by the sensor in beats per minute.</summary>
            public byte ComputedHeartRate { get; }
            /// <summary>RR interval in milliseconds.</summary>
            public int RRInterval { get; }

            internal CommonHeartRateData(int accumEventTime, byte heartRate, int rrInverval)
            {
                AccumulatedHeartBeatEventTime = accumEventTime * 1000 / 1024;
                ComputedHeartRate = heartRate;
                RRInterval = rrInverval;
            }
        }

        /// <summary>
        /// Manufacturer supplied info. It may be sent as a main page or background page.
        /// </summary>
        public readonly struct ManufacturerInfoPage
        {
            /// <summary>The manufacturing identifier LSB</summary>
            public byte ManufacturingIdLsb { get; }
            /// <summary>The serial number</summary>
            public uint SerialNumber { get; }

            internal ManufacturerInfoPage(byte[] dataPage, uint deviceNumber)
            {
                ManufacturingIdLsb = dataPage[1];
                SerialNumber = (uint)((BitConverter.ToUInt16(dataPage, 2) << 16) + (deviceNumber & 0x0000FFFF));
            }
        }

        /// <summary>
        /// Product information. Sent as a background page. All fields are manufacturer specific.
        /// </summary>
        public readonly struct ProductInfoPage
        {
            /// <summary>Gets the hardware version.</summary>
            public byte HardwareVersion { get; }
            /// <summary>Gets the software version.</summary>
            public byte SoftwareVersion { get; }
            /// <summary>Gets the model number.</summary>
            public byte ModelNumber { get; }

            internal ProductInfoPage(byte[] dataPage)
            {
                HardwareVersion = dataPage[1];
                SoftwareVersion = dataPage[2];
                ModelNumber = dataPage[3];
            }
        }

        /// <summary>
        /// This structure provides the RR interval and manufacturer specific data. It is typically sent as a main page.
        /// </summary>
        public readonly struct PreviousHeartBeatPage
        {
            /// <summary>Manufacturer specific data. Set to 0xFF if not used.</summary>
            public byte ManufacturerSpecific { get; }
            /// <summary>RR interval in milliseconds.</summary>
            public int RRInterval { get; }

            internal PreviousHeartBeatPage(byte[] dataPage)
            {
                ManufacturerSpecific = dataPage[1];
                RRInterval = CalculateRRInverval(BitConverter.ToUInt16(dataPage, 2), BitConverter.ToUInt16(dataPage, 4));
            }
        }

        /// <summary>
        /// Swim interval data
        /// </summary>
        public readonly struct SwimIntervalPage
        {
            /// <summary>Swim interval average heart rate.</summary>
            public byte IntervalAverageHeartRate { get; }
            /// <summary>Swim interval maximum heart rate.</summary>
            public byte IntervalMaximumHeartRate { get; }
            /// <summary>Swim session average heart rate.</summary>
            public byte SessionAverageHeartRate { get; }

            internal SwimIntervalPage(byte[] dataPage)
            {
                IntervalAverageHeartRate = dataPage[1];
                IntervalMaximumHeartRate = dataPage[2];
                SessionAverageHeartRate = dataPage[3];
            }
        }

        /// <summary>
        /// Heart rate device capabilities
        /// </summary>
        public readonly struct CapabilitiesPage
        {
            /// <summary>Enabled features.</summary>
            public Features Enabled { get; }
            /// <summary>Supported features.</summary>
            public Features Supported { get; }

            internal CapabilitiesPage(byte[] dataPage)
            {
                Supported = (Features)(dataPage[2] & (byte)Features.All);
                Enabled = (Features)(dataPage[3] & (byte)Features.All);
            }
        }

        /// <summary>
        /// The battery status page. Sent as a background page.
        /// </summary>
        public readonly struct BatteryStatusPage
        {
            /// <summary>Battery level.</summary>
            public byte BatteryLevel { get; }
            /// <summary>Battery voltage.</summary>
            public double BatteryVoltage { get; }
            /// <summary>Battery status.</summary>
            public BatteryStatus BatteryStatus { get; }

            internal BatteryStatusPage(byte[] dataPage)
            {
                BatteryLevel = dataPage[1];
                BatteryVoltage = (dataPage[3] & 0x0F) + (dataPage[2] / 256.0);
                BatteryStatus = (BatteryStatus)((dataPage[3] & 0x70) >> 4);
            }
        }

        /// <summary>
        /// This structure provides manufacturer specific data. Sent as a background page.
        /// </summary>
        public readonly struct ManufacturerSpecificPage
        {
            /// <summary>The manufacturer specific page number.</summary>
            public byte Page { get; }
            /// <summary>The manufacturer specific data.</summary>
            public byte[] Data { get; }

            internal ManufacturerSpecificPage(byte[] dataPage)
            {
                Page = dataPage[0];
                Data = dataPage.Skip(1).Take(3).ToArray();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CommonHeartRateData HeartRateData { get; private set; }
        public TimeSpan CumulativeOperatingTime { get; private set; }
        public HeartbeatEventType EventType { get; private set; }
        public ManufacturerInfoPage ManufacturerInfo { get; private set; }
        public ProductInfoPage ProductInfo { get; private set; }
        public PreviousHeartBeatPage PreviousHeartBeat { get; private set; }
        public SwimIntervalPage SwimInterval { get; private set; }
        public CapabilitiesPage Capabilities { get; private set; }
        public BatteryStatusPage BatteryStatus { get; private set; }
        public ManufacturerSpecificPage ManufacturerSpecific { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="HeartRate" /> class.</summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel"></param>
        public HeartRate(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
        }

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public override void Parse(byte[] dataPage)
        {
            if (isFirstDataMessage)
            {
                isFirstDataMessage = false;
                observedToggle = dataPage[0] & 0x80;
                prevBeatEventTime = BitConverter.ToUInt16(dataPage, 4);
                prevBeatCount = dataPage[6];
                lastDataPage = dataPage;
                HeartRateData = new CommonHeartRateData(accumHeartBeatEventTime, dataPage[7], rrInterval);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HeartRateData)));
                return;
            }

            // ignore duplicate/unchanged data pages
            if (lastDataPage.SequenceEqual(dataPage))
            {
                return;
            }
            lastDataPage = dataPage;

            // this data is present in all data pages
            // fire heart rate event if beat count has changed
            int deltaHeartBeatCount = Utils.CalculateDelta(dataPage[6], ref prevBeatCount);
            if (deltaHeartBeatCount > 0)
            {
                // calculate RR interval if delta beat count is 1
                if (deltaHeartBeatCount == 1)
                {
                    rrInterval = CalculateRRInverval(prevBeatEventTime, BitConverter.ToUInt16(dataPage, 4));
                }

                accumHeartBeatEventTime += Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 4), ref prevBeatEventTime);
                HeartRateData = new CommonHeartRateData(accumHeartBeatEventTime, dataPage[7], rrInterval);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HeartRateData)));
            }

            // handle data page toggle
            if (!pageToggle)
            {
                pageToggle = (dataPage[0] & 0x80) != observedToggle;
                if (!pageToggle) return;
            }

            switch ((DataPage)(dataPage[0] & 0x7F))
            {
                case DataPage.CumulativeOperatingTime:
                    CumulativeOperatingTime = TimeSpan.FromSeconds((BitConverter.ToUInt32(dataPage, 1) & 0x00FFFFFF) * 2.0);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CumulativeOperatingTime)));
                    break;
                case DataPage.ManufacturerInfo:
                    ManufacturerInfo = new ManufacturerInfoPage(dataPage, ChannelId.DeviceNumber);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ManufacturerInfo)));
                    break;
                case DataPage.ProductInfo:
                    ProductInfo = new ProductInfoPage(dataPage);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProductInfo)));
                    break;
                case DataPage.PreviousHeartBeat:
                    // fire event if beat count has changed
                    if (deltaHeartBeatCount > 0)
                    {
                        PreviousHeartBeat = new PreviousHeartBeatPage(dataPage);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PreviousHeartBeat)));
                    }
                    break;
                case DataPage.SwimInterval:
                    SwimInterval = new SwimIntervalPage(dataPage);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SwimInterval)));
                    break;
                case DataPage.Capabilities:
                    Capabilities = new CapabilitiesPage(dataPage);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Capabilities)));
                    break;
                case DataPage.BatteryStatus:
                    BatteryStatus = new BatteryStatusPage(dataPage);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BatteryStatus)));
                    break;
                case DataPage.DeviceInformation:
                    EventType = (HeartbeatEventType)(dataPage[1] & 0x03);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EventType)));
                    break;
                default:
                    // range check manufacturer specific pages
                    if (dataPage[0] >= 112 && dataPage[0] < 128)
                    {
                        // let application parse
                        ManufacturerSpecific = new ManufacturerSpecificPage(dataPage);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ManufacturerSpecific)));
                    }
                    break;
            }
        }

        /// <summary>
        /// Sets the sport mode.
        /// </summary>
        /// <param name="sportMode">The sport mode.</param>
        public void SetSportMode(SportMode sportMode, SubSportMode subSportMode = SubSportMode.None)
        {
            SendExtAcknowledgedMessage(CommonDataPages.FormatModeSettingsPage(sportMode, subSportMode));
        }

        /// <summary>
        /// Updates the heart rate sensor feature. The HR Feature command page is sent from a display to a heart rate monitor when the display
        /// wants to update the enabled status of a HR feature.
        /// </summary>
        /// <remarks>
        /// Gym mode helps a single group receiver running a continuous scan differentiate between many
        /// transmitting heart rate monitors; a common problem encountered in gym-based group fitness
        /// applications.This is achieved by transmitting data page 2 as a main data page.
        /// </remarks>
        /// <param name="applyGymMode">if set to <c>true</c> apply gym mode. Displays cannot rely on this field as older sensors do not decode it. The Gym Mode bit shall be set to the last received value from the capabilities page if Apply Gym Mode is set to false.</param>
        /// <param name="gymMode">if set to <c>true</c> gym mode is enabled.</param>
        public void SetHRFeature(bool applyGymMode, bool gymMode)
        {
            byte[] msg = new byte[] { (byte)DataPage.HRFeature, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, (byte)(applyGymMode ? 0xFF : 0x7F), (byte)(gymMode ? 0xFF : 0x7F) };
            SendExtAcknowledgedMessage(msg);
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

        public override void ChannelEventHandler(EventMsgId eventMsgId)
        {
            throw new NotImplementedException();
        }

        public override void ChannelResponseHandler(byte messageId, ResponseMsgId responseMsgId)
        {
            throw new NotImplementedException();
        }
    }
}
