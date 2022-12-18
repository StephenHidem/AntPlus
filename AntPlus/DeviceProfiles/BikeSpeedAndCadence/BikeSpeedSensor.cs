using AntRadioInterface;
using System;
using System.Linq;

namespace AntPlus.DeviceProfiles.BikeSpeedAndCadence
{
    internal class BikeSpeedSensor : AntDevice
    {
        /// <summary>
        /// The BikeSpeedSensor device class ID.
        /// </summary>
        public const byte DeviceClass = 123;

        /// <summary>
        /// Bike speed data pages.
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
            /// <summary>Battery status</summary>
            BatteryStatus,
            /// <summary>Motion and speed</summary>
            MotionAndSpeed,
        }

        private bool isFirstDataMessage = true;     // used for accumulated values
        private ushort prevEventTime;
        private ushort prevRevCount;
        private bool pageToggle = false;
        private int observedToggle;

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


        /// <summary>Occurs when cumulative operating time page changed.</summary>
        public event EventHandler<TimeSpan> CumulativeOperatingTimePageChanged;
        /// <summary>Occurs when manufacturer information page changed.</summary>
        public event EventHandler<ManufacturerInfoPage> ManufacturerInfoPageChanged;
        /// <summary>Occurs when product information page changed.</summary>
        public event EventHandler<ProductInfoPage> ProductInfoPageChanged;
        /// <summary>Occurs when battery status page changed.</summary>
        public event EventHandler<BatteryStatusPage> BatteryStatusPageChanged;
        /// <summary>Occurs when stop indicator has changed.</summary>
        public event EventHandler<bool> StopIndicatorChanged;
        public event EventHandler BikeSpeedSensorChanged;

        public double WheelCircumference { get; set; } = 2.2;
        public double InstantaneousSpeed { get; private set; }
        public double AccumulatedDistance { get; private set; }

        public BikeSpeedSensor(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
        }

        public override void Parse(byte[] dataPage)
        {
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

            // this data is present in all data pages
            int deltaEventTime = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 4), ref prevEventTime);
            int deltaRevCount = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 6), ref prevRevCount);
            InstantaneousSpeed = WheelCircumference * deltaRevCount * 1024.0 / deltaEventTime;
            AccumulatedDistance = WheelCircumference * deltaRevCount;
            BikeSpeedSensorChanged?.Invoke(this, EventArgs.Empty);

            // handle data page toggle
            if (!pageToggle)
            {
                pageToggle = (dataPage[0] & 0x80) != observedToggle;
                if (!pageToggle) return;
            }

            switch ((DataPage)dataPage[0])
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
                case DataPage.BatteryStatus:
                    BatteryStatusPageChanged?.Invoke(this, new BatteryStatusPage(dataPage));
                    break;
                case DataPage.MotionAndSpeed:
                    StopIndicatorChanged?.Invoke(this, dataPage[1] == 0x01);
                    break;
                default:
                    break;
            }
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
