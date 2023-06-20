using SmallEarthTech.AntRadioInterface;
using System;
using System.Linq;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence
{
    /// <summary>
    /// This class is common to speed and cadence sensors. Combined speed and cadence sensors do not use this class.
    /// </summary>
    /// <seealso cref="AntDevice" />
    public abstract class CommonSpeedCadence : AntDevice
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
            /// <summary>Cumulative operating time</summary>
            CumulativeOperatingTime = 1,
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
        public TimeSpan CumulativeOperatingTime { get; private set; }
        /// <summary>Gets the manufacturer information.</summary>
        public ManufacturerInfoPage ManufacturerInfo { get; private set; }
        /// <summary>Gets the product information.</summary>
        public ProductInfoPage ProductInfo { get; private set; }
        /// <summary>Gets the battery status.</summary>
        public BatteryStatusPage BatteryStatus { get; private set; }
        /// <summary>Gets a value indicating whether this sensor has detected stopped condition.</summary>
        public bool Stopped { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonSpeedCadence"/> class.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        /// <param name="timeout">Time in milliseconds before firing <see cref="AntDevice.DeviceWentOffline"/>.</param>
        public CommonSpeedCadence(ChannelId channelId, IAntChannel antChannel, int timeout = 2000) : base(channelId, antChannel, timeout)
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

            switch ((DataPage)dataPage[0])
            {
                case DataPage.CumulativeOperatingTime:
                    CumulativeOperatingTime = TimeSpan.FromSeconds((BitConverter.ToUInt32(dataPage, 1) & 0x00FFFFFF) * 2.0);
                    RaisePropertyChange(nameof(CumulativeOperatingTime));
                    break;
                case DataPage.ManufacturerInfo:
                    ManufacturerInfo = new ManufacturerInfoPage(dataPage, ChannelId.DeviceNumber);
                    RaisePropertyChange(nameof(ManufacturerInfo));
                    break;
                case DataPage.ProductInfo:
                    ProductInfo = new ProductInfoPage(dataPage);
                    RaisePropertyChange(nameof(ProductInfo));
                    break;
                case DataPage.BatteryStatus:
                    BatteryStatus = new BatteryStatusPage(dataPage);
                    RaisePropertyChange(nameof(BatteryStatus));
                    break;
                case DataPage.Motion:
                    Stopped = dataPage[1] == 0x01;
                    RaisePropertyChange(nameof(Stopped));
                    break;
                default:
                    break;
            }
        }
    }
}
