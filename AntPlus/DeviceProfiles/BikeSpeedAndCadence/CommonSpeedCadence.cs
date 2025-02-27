using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Linq;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence
{
    /// <summary>
    /// This class is common to speed and cadence sensors. Combined speed and cadence sensors do not use this class.
    /// </summary>
    /// <seealso cref="AntDevice" />
    public abstract partial class CommonSpeedCadence : AntDevice
    {
        /// <summary>The is first data message
        /// received.</summary>
        protected bool isFirstDataMessage = true;     // used for accumulated values
        private byte[] lastDataPage = new byte[8];
        private bool pageToggle;
        private int observedToggle;
        /// <summary>The previous event time</summary>
        protected ushort prevEventTime;
        /// <summary>The previous revolution count</summary>
        protected ushort prevRevCount;

        /// <summary>
        /// Bike speed/cadence data pages.
        /// </summary>
        public enum DataPage
        {
            /// <summary>Default or unknown page</summary>
            Default,
            /// <summary>Cumulative operating time</summary>
            CumulativeOperatingTime,
            /// <summary>Manufacturer information</summary>
            ManufacturerInfo,
            /// <summary>Product information</summary>
            ProductInfo,
            /// <summary>Battery status</summary>
            BatteryStatus,
            /// <summary>Motion</summary>
            Motion,
        }

        /// <summary>
        /// Manufacturer supplied info.
        /// </summary>
        public readonly struct ManufacturerInfoPage
        {
            /// <summary>The manufacturing identifier</summary>
            public byte ManufacturingId { get; }
            /// <summary>The serial number</summary>
            public uint SerialNumber { get; }

            internal ManufacturerInfoPage(byte[] dataPage, uint deviceNumber)
            {
                ManufacturingId = dataPage[1];
                SerialNumber = (uint)((BitConverter.ToUInt16(dataPage, 2) << 16) + (deviceNumber & 0x0000FFFF));
            }
        }

        /// <summary>
        /// Product information.
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
        /// The battery status page.
        /// </summary>
        public readonly struct BatteryStatusPage
        {
            /// <summary>Battery voltage.</summary>
            public double BatteryVoltage { get; }
            /// <summary>Battery status.</summary>
            public BatteryStatus BatteryStatus { get; }

            internal BatteryStatusPage(byte[] dataPage)
            {
                BatteryVoltage = (dataPage[3] & 0x0F) + (dataPage[2] / 256.0);
                BatteryStatus = (BatteryStatus)((dataPage[3] & 0x70) >> 4);
            }
        }

        /// <summary>Gets the cumulative operating time.</summary>
        [ObservableProperty]
        private TimeSpan cumulativeOperatingTime;
        /// <summary>Gets the manufacturer information.</summary>
        [ObservableProperty]
        private ManufacturerInfoPage manufacturerInfo;
        /// <summary>Gets the product information.</summary>
        [ObservableProperty]
        private ProductInfoPage productInfo;
        /// <summary>Gets the battery status.</summary>
        [ObservableProperty]
        private BatteryStatusPage batteryStatus;
        /// <summary>Gets a value indicating whether this sensor has detected stopped condition.</summary>
        [ObservableProperty]
        private bool stopped;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonSpeedCadence"/> class.
        /// </summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, int)"/>
        public CommonSpeedCadence(ChannelId channelId, IAntChannel antChannel, ILogger logger, int timeout)
            : base(channelId, antChannel, logger, timeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonSpeedCadence"/> class.
        /// </summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, TimeoutOptions?)"/>
        public CommonSpeedCadence(ChannelId channelId, IAntChannel antChannel, ILogger logger, TimeoutOptions? timeoutOptions)
            : base(channelId, antChannel, logger, timeoutOptions)
        {
        }

        /// <inheritdoc/>
        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);

            if (isFirstDataMessage)
            {
                isFirstDataMessage = false;
                observedToggle = dataPage[0] & 0x80;
                prevEventTime = BitConverter.ToUInt16(dataPage, 4);
                prevRevCount = BitConverter.ToUInt16(dataPage, 6);
                lastDataPage = dataPage;
                return;
            }

            // ignore duplicate/unchanged data pages
            if (lastDataPage.SequenceEqual(dataPage))
            {
                return;
            }
            lastDataPage = dataPage;

            // handle data page toggle
            if (!pageToggle)
            {
                pageToggle = (dataPage[0] & 0x80) != observedToggle;
                if (!pageToggle) return;
            }

            switch ((DataPage)(dataPage[0] & 0x7F))
            {
                case DataPage.Default:
                    // the default page is handled by the derived classes
                    break;
                case DataPage.CumulativeOperatingTime:
                    CumulativeOperatingTime = TimeSpan.FromSeconds((BitConverter.ToUInt32(dataPage, 1) & 0x00FFFFFF) * 2.0);
                    break;
                case DataPage.ManufacturerInfo:
                    ManufacturerInfo = new ManufacturerInfoPage(dataPage, ChannelId.DeviceNumber);
                    break;
                case DataPage.ProductInfo:
                    ProductInfo = new ProductInfoPage(dataPage);
                    break;
                case DataPage.BatteryStatus:
                    BatteryStatus = new BatteryStatusPage(dataPage);
                    break;
                case DataPage.Motion:
                    Stopped = dataPage[1] == 0x01;
                    break;
                default:
                    _logger.UnknownDataPage(dataPage);
                    break;
            }
        }
    }
}
