using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace SmallEarthTech.AntPlus
{
    /// <summary>Supported common data pages.</summary>
    public enum CommonDataPage
    {
        /// <summary>Request data page</summary>
        RequestDataPage = 0x46,
        /// <summary>Command status data page</summary>
        CommandStatus = 0x47,
        /// <summary>Generic command data page</summary>
        GenericCommandPage = 0x49,
        /// <summary>Open channel command data page</summary>
        OpenChannelCommand = 0x4A,
        /// <summary>Mode settings data page</summary>
        ModeSettingsPage = 0x4C,
        /// <summary>Multiple component manufacturer info data page</summary>
        MultiComponentManufacturerInfo = 0x4E,
        /// <summary>Multiple component product info data page</summary>
        MultiComponentProductInfo = 0x4F,
        /// <summary>Manufacturer info data page</summary>
        ManufacturerInfo = 0x50,
        /// <summary>Product info data page</summary>
        ProductInfo = 0x51,
        /// <summary>Battery status data page</summary>
        BatteryStatus = 0x52,
        /// <summary>Time and date data page</summary>
        TimeAndDate = 0x53,
        /// <summary>Subfield data page</summary>
        SubfieldData = 0x54,
        /// <summary>Memory level data page</summary>
        MemoryLevel = 0x55,
        /// <summary>Paired devices data page</summary>
        PairedDevices = 0x56,
        /// <summary>Error description data page</summary>
        ErrorDescription = 0x57
    }
    /// <summary>Data page command type.</summary>
    public enum CommandType
    {
        /// <summary>Unknown command</summary>
        Unknown,
        /// <summary>Data page command</summary>
        DataPage,
        /// <summary>ANT-FS command</summary>
        AntFSSession,
        /// <summary>Data page from slave command</summary>
        DataPageFromSlave,
        /// <summary>Data page set command</summary>
        DataPageSet
    }
    /// <summary>Battery status description.</summary>
    public enum BatteryStatus
    {
        /// <summary>Unknown</summary>
        Unknown,
        /// <summary>New</summary>
        New,
        /// <summary>Good</summary>
        Good,
        /// <summary>Ok</summary>
        Ok,
        /// <summary>Low</summary>
        Low,
        /// <summary>Critical</summary>
        Critical,
        /// <summary>Reserved</summary>
        Reserved,
        /// <summary>Invalid</summary>
        Invalid
    }
    /// <summary>
    /// Use this enumeration to set the desired sport mode.
    /// </summary>
    public enum SportMode
    {
        /// <summary>Default behavior</summary>
        Generic,
        /// <summary>Running</summary>
        Running,
        /// <summary>Cycling</summary>
        Cycling,
        /// <summary>Swimming</summary>
        Swimming = 5,
    }
    /// <summary>Currently the sub sport mode is only used by heart rate monitors.</summary>
    public enum SubSportMode
    {
        /// <summary>Generic. Use default.</summary>
        Generic = 0,
        /// <summary>Treadmill.</summary>
        Treadmill = 1,
        /// <summary>Spin class.</summary>
        Spin = 5,
        /// <summary>Lap swimming.</summary>
        LapSwimming = 17,
        /// <summary>Not used.</summary>
        None = 0xFF
    }

    /// <summary>
    /// This class supports common data pages.  In particular, this class describes the common pages used by ANT+ devices.
    /// </summary>
    public partial class CommonDataPages : ObservableObject
    {
        private readonly ILogger _logger;

        /// <summary>Command result.</summary>
        public enum CommandResult
        {
            /// <summary>Pass.</summary>
            Pass,
            /// <summary>Fail.</summary>
            Fail,
            /// <summary>Not supported.</summary>
            NotSupported,
            /// <summary>Rejected.</summary>
            Rejected,
            /// <summary>Pending.</summary>
            Pending
        }
        /// <summary>Memory size unit used.</summary>
        public enum MemorySizeUnit
        {
            /// <summary>In bits.</summary>
            Bits = 0x00,
            /// <summary>In kilobits.</summary>
            KiloBits = 0x01,
            /// <summary>In megabits.</summary>
            MegaBits = 0x02,
            /// <summary>In terabits.</summary>
            TeraBits = 0x03,
            /// <summary>In bytes.</summary>
            Bytes = 0x80,
            /// <summary>In kilobytes.</summary>
            KiloBytes = 0x81,
            /// <summary>In megabytes.</summary>
            MegaBytes = 0x82,
            /// <summary>In terabytes.</summary>
            TeraBytes = 0x83
        }
        /// <summary>Severity of error.</summary>
        public enum ErrorLevel
        {
            /// <summary>Unknown.</summary>
            Unknown = 0,
            /// <summary>Warning.</summary>
            Warning = 1,
            /// <summary>Critical.</summary>
            Critical = 2,
            /// <summary>Reserved.</summary>
            Reserved = 3
        }

        /// <summary>The command status page.</summary>
        public readonly struct CommandStatusPage
        {
            /// <summary>Gets the last command received.</summary>
            public byte LastCommandReceived { get; }
            /// <summary>Gets the sequence number of the last command.</summary>
            public byte SequenceNumber { get; }
            /// <summary>Gets the status of the last command.</summary>
            public CommandResult Status { get; }
            /// <summary>Gets the response of the last command.</summary>
            public uint ResponseData { get; }

            internal CommandStatusPage(byte[] dataPage)
            {
                LastCommandReceived = dataPage[1];
                SequenceNumber = dataPage[2];
                Status = (CommandResult)dataPage[3];
                ResponseData = BitConverter.ToUInt32(dataPage, 4);
            }
        }
        /// <summary>Manufacturer info page.</summary>
        public readonly struct ManufacturerInfoPage
        {
            /// <summary>Gets the hardware revision.</summary>
            public byte HardwareRevision { get; }
            /// <summary>Gets the manufacturer identifier.</summary>
            public ushort ManufacturerId { get; }
            /// <summary>Gets the model number.</summary>
            public ushort ModelNumber { get; }

            internal ManufacturerInfoPage(byte[] dataPage)
            {
                HardwareRevision = dataPage[3];
                ManufacturerId = BitConverter.ToUInt16(dataPage, 4);
                ModelNumber = BitConverter.ToUInt16(dataPage, 6);
            }
        }
        /// <summary>Product info page.</summary>
        public readonly struct ProductInfoPage
        {
            /// <summary>Gets the software revision.</summary>
            public Version SoftwareRevision { get; }
            /// <summary>Gets the serial number.</summary>
            public uint SerialNumber { get; }

            internal ProductInfoPage(byte[] dataPage)
            {
                // create a number format info provider with a decimal separator of '.'
                System.Globalization.NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." };
                if (dataPage[2] != 0xFF)
                {
                    // supplemental SW revision is valid
                    SoftwareRevision = Version.Parse(((dataPage[3] * 100.0 + dataPage[2]) / 1000.0).ToString("N3", nfi));
                }
                else
                {
                    // only main SW revision is present
                    SoftwareRevision = Version.Parse((dataPage[3] / 10.0).ToString("N3", nfi));
                }
                SerialNumber = BitConverter.ToUInt32(dataPage, 4);
            }
        }
        /// <summary>Battery status page.</summary>
        public readonly struct BatteryStatusPage
        {
            /// <summary>Gets the number of batteries.</summary>
            public byte NumberOfBatteries { get; }
            /// <summary>Gets the battery identifier index.</summary>
            public byte Identifier { get; }
            /// <summary>Gets the cumulative operating time.</summary>
            public TimeSpan CumulativeOperatingTime { get; }
            /// <summary>Gets the battery status.</summary>
            public BatteryStatus Status { get; }
            /// <summary>Gets the battery voltage.</summary>
            public double BatteryVoltage { get; }

            internal BatteryStatusPage(byte[] dataPage)
            {
                if (dataPage[2] != 0xFF)
                {
                    NumberOfBatteries = (byte)(dataPage[2] & 0x0F);
                    Identifier = (byte)(dataPage[2] >> 4);
                }
                else
                {
                    NumberOfBatteries = 1; Identifier = 0;
                }
                CumulativeOperatingTime =
                    TimeSpan.FromSeconds((BitConverter.ToInt32(dataPage, 3) & 0x00FFFFFF) * (((dataPage[7] & 0x80) == 0x80) ? 2.0 : 16.0));
                BatteryVoltage = (dataPage[7] & 0x0F) + (dataPage[6] / 256.0);
                Status = (BatteryStatus)((dataPage[7] & 0x70) >> 4);
            }
        }
        /// <summary>Subfield data page.</summary>
        public readonly struct SubfieldDataPage
        {
            /// <summary>The subfield data page type.</summary>
            public enum SubPage
            {
                /// <summary>The temperature in degrees Celsius.</summary>
                Temperature = 1,
                /// <summary>The barometric pressure in kPa.</summary>
                BarometricPressure,
                /// <summary>The percent air humidity</summary>
                Humidity,
                /// <summary>The wind speed in kilometers per hour.</summary>
                WindSpeed,
                /// <summary>The wind direction in degrees.</summary>
                WindDirection,
                /// <summary>The number of charging cycles.</summary>
                ChargingCycles,
                /// <summary>The minimum operating temperature in degrees Celsius.</summary>
                MinimumOperatingTemperature,
                /// <summary>The maximum operating temperature in degrees Celsius.</summary>
                MaximumOperatingTemperature,
                /// <summary>Invalid.</summary>
                Invalid = 0xFF
            }

            /// <summary>SubPage data field.</summary>
            public SubPage Subpage1 { get; }
            /// <summary>Gets the computed data field 1. Returns NaN if this is not a valid subpage.</summary>
            public double ComputedDataField1 { get; }
            /// <summary>SubPage data field.</summary>
            public SubPage Subpage2 { get; }
            /// <summary>Gets the computed data field 2. Returns NaN if this is not a valid subpage.</summary>
            public double ComputedDataField2 { get; }

            internal SubfieldDataPage(byte[] dataPage)
            {
                Subpage1 = (SubPage)dataPage[2];
                Subpage2 = (SubPage)dataPage[3];
                ComputedDataField1 = ParseSubfieldData(Subpage1, BitConverter.ToInt16(dataPage, 4));
                ComputedDataField2 = ParseSubfieldData(Subpage2, BitConverter.ToInt16(dataPage, 6));
            }

            private static double ParseSubfieldData(SubPage page, short value)
            {
                double retVal = double.NaN;
                switch (page)
                {
                    case SubPage.Temperature:
                        retVal = value * 0.01;
                        break;
                    case SubPage.BarometricPressure:
                        retVal = (ushort)value * 0.01;
                        break;
                    case SubPage.Humidity:
                        retVal = value / 100.0;
                        break;
                    case SubPage.WindSpeed:
                        retVal = (ushort)value * 0.01;
                        break;
                    case SubPage.WindDirection:
                        retVal = value / 20.0;
                        break;
                    case SubPage.ChargingCycles:
                        retVal = (ushort)value;
                        break;
                    case SubPage.MinimumOperatingTemperature:
                        retVal = value / 100.0;
                        break;
                    case SubPage.MaximumOperatingTemperature:
                        retVal = value / 100.0;
                        break;
                    default:
                        break;
                }
                return retVal;
            }
        }
        /// <summary>The memory usage page.</summary>
        public readonly struct MemoryLevelPage
        {
            /// <summary>Gets the percent of memory used.</summary>
            public double PercentUsed { get; }
            /// <summary>Gets the total size of memory.</summary>
            public double TotalSize { get; }
            /// <summary>Gets the unit used.</summary>
            public MemorySizeUnit TotalSizeUnit { get; }

            internal MemoryLevelPage(byte[] dataPage)
            {
                PercentUsed = dataPage[4] * 0.5;
                TotalSize = BitConverter.ToUInt16(dataPage, 5) * 0.1;
                TotalSizeUnit = (MemorySizeUnit)(dataPage[7] & 0x83);
            }
        }
        /// <summary>Error description structure.</summary>
        public readonly struct ErrorDescriptionPage
        {
            /// <summary>Gets the index of the system component.</summary>
            public byte SystemComponentIndex { get; }
            /// <summary>Gets the error level.</summary>
            public ErrorLevel ErrorLevel { get; }
            /// <summary>Gets the profile specific error code.</summary>
            public byte ProfileSpecificErrorCode { get; }
            /// <summary>Gets the manufacturer specific error code.</summary>
            public uint ManufacturerSpecificErrorCode { get; }

            internal ErrorDescriptionPage(byte[] dataPage)
            {
                SystemComponentIndex = (byte)(dataPage[2] & 0x0F);
                ErrorLevel = (ErrorLevel)((dataPage[2] & 0xC0) >> 6);
                ProfileSpecificErrorCode = dataPage[3];
                ManufacturerSpecificErrorCode = BitConverter.ToUInt32(dataPage, 4);
            }
        }

        /// <summary>Gets the command status.</summary>
        [ObservableProperty]
        private CommandStatusPage commandStatus;
        /// <summary>Gets the manufacturer information.</summary>
        [ObservableProperty]
        private ManufacturerInfoPage manufacturerInfo;
        /// <summary>Gets the product information.</summary>
        [ObservableProperty]
        private ProductInfoPage productInfo;
        /// <summary>Gets the battery status.</summary>
        [ObservableProperty]
        private BatteryStatusPage batteryStatus;
        /// <summary>Gets the time and date.</summary>
        [ObservableProperty]
        private DateTime timeAndDate;
        /// <summary>Gets the subfield data.</summary>
        [ObservableProperty]
        private SubfieldDataPage subfieldData;
        /// <summary>Gets the memory level.</summary>
        [ObservableProperty]
        private MemoryLevelPage memoryLevel;
        /// <summary>Gets the error description.</summary>
        [ObservableProperty]
        private ErrorDescriptionPage errorDescription;

        /// <summary>Initializes a new instance of the <see cref="CommonDataPages" /> class.</summary>
        /// <param name="logger">The logger.</param>
        public CommonDataPages(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>Parses the common data page.</summary>
        /// <param name="dataPage">The data page.</param>
        public void ParseCommonDataPage(byte[] dataPage)
        {
            switch ((CommonDataPage)dataPage[0])
            {
                case CommonDataPage.CommandStatus:
                    CommandStatus = new CommandStatusPage(dataPage);
                    break;
                case CommonDataPage.MultiComponentManufacturerInfo:
                    break;
                case CommonDataPage.MultiComponentProductInfo:
                    break;
                case CommonDataPage.ManufacturerInfo:
                    ManufacturerInfo = new ManufacturerInfoPage(dataPage);
                    break;
                case CommonDataPage.ProductInfo:
                    ProductInfo = new ProductInfoPage(dataPage);
                    break;
                case CommonDataPage.BatteryStatus:
                    BatteryStatus = new BatteryStatusPage(dataPage);
                    break;
                case CommonDataPage.TimeAndDate:
                    TimeAndDate = new DateTime(2000 + dataPage[7], dataPage[6], dataPage[5] & 0x1F, dataPage[4], dataPage[3], dataPage[2], DateTimeKind.Utc);
                    break;
                case CommonDataPage.SubfieldData:
                    SubfieldData = new SubfieldDataPage(dataPage);
                    break;
                case CommonDataPage.MemoryLevel:
                    MemoryLevel = new MemoryLevelPage(dataPage);
                    break;
                case CommonDataPage.PairedDevices:
                    break;
                case CommonDataPage.ErrorDescription:
                    ErrorDescription = new ErrorDescriptionPage(dataPage);
                    break;
                default:
                    _logger.LogWarning("{Func}: unknown data page. Page = {Page}", nameof(ParseCommonDataPage), BitConverter.ToString(dataPage));
                    break;
            }
        }

        /// <summary>Formats the generic command page.</summary>
        /// <param name="slaveSerialNumber">The slave serial number.</param>
        /// <param name="slaveManufacturerId">The slave manufacturer identifier.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <param name="command">The command.</param>
        /// <returns>The formatted command page.</returns>
        public static byte[] FormatGenericCommandPage(ushort slaveSerialNumber, ushort slaveManufacturerId, byte sequenceNumber, ushort command)
        {
            byte[] msg = new byte[] { (byte)CommonDataPage.GenericCommandPage }.
                Concat(BitConverter.GetBytes(slaveSerialNumber)).
                Concat(BitConverter.GetBytes(slaveManufacturerId)).
                Concat(new byte[] { sequenceNumber }).
                Concat(BitConverter.GetBytes(command)).
                ToArray();
            return msg;
        }

        /// <summary>Formats the open channel command page.</summary>
        /// <param name="slaveSerialNumber">The slave serial number.</param>
        /// <param name="deviceType">Type of the device.</param>
        /// <param name="frequency">The frequency.</param>
        /// <param name="channelPeriod">The channel period.</param>
        /// <returns>The formatted data page.</returns>
        public static byte[] FormatOpenChannelCommandPage(uint slaveSerialNumber, byte deviceType, byte frequency, ushort channelPeriod)
        {
            byte[] msg = new byte[] { (byte)CommonDataPage.OpenChannelCommand }.
                Concat(BitConverter.GetBytes(slaveSerialNumber).Take(3)).
                Concat(new byte[] { deviceType }).
                Concat(new byte[] { frequency }).
                Concat(BitConverter.GetBytes(channelPeriod)).
                ToArray();
            return msg;
        }

        /// <summary>Formats the mode settings page.</summary>
        /// <param name="sportMode">The sport mode.</param>
        /// <param name="subSportMode">The sub sport mode.
        /// Currently used only by heart rate monitors,</param>
        /// <returns>The formatted data page.</returns>
        public static byte[] FormatModeSettingsPage(SportMode sportMode, SubSportMode subSportMode = SubSportMode.None)
        {
            return new byte[] { (byte)CommonDataPage.ModeSettingsPage, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, (byte)subSportMode, (byte)sportMode };
        }
    }
}
