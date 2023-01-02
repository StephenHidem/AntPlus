using System;
using System.ComponentModel;

namespace AntPlus
{
    public enum CommonDataPage
    {
        RequestDataPage = 0x46,
        CommandStatus = 0x47,
        GenericCommandPage = 0x49,
        OpenChannelCommand = 0x4A,
        ModeSettingsPage = 0x4C,
        MultiComponentManufacturerInfo = 0x4E,
        MultiComponentProductInfo = 0x4F,
        ManufacturerInfo = 0x50,
        ProductInfo = 0x51,
        BatteryStatus = 0x52,
        TimeAndDate = 0x53,
        SubfieldData = 0x54,
        MemoryLevel = 0x55,
        PairedDevices = 0x56,
        ErrorDescription = 0x57
    }
    public enum CommandType
    {
        Unknown,
        DataPage,
        AntFSSesion,
        DataPageFromSlave,
        DataPageSet
    }
    public enum BatteryStatus
    {
        Unknown,
        New,
        Good,
        Ok,
        Low,
        Critical,
        Reserved,
        Invalid
    }

    public class CommonDataPages2 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public enum CommandResult
        {
            Pass,
            Fail,
            NotSupported,
            Rejected,
            Pending
        }
        public enum MemorySizeUnit
        {
            Bits = 0x00,
            KiloBits = 0x01,
            MegaBits = 0x02,
            TeraBits = 0x03,
            Bytes = 0x80,
            KiloBytes = 0x81,
            MegaBytes = 0x82,
            TeraBytes = 0x83
        }
        public enum ErrorLevel
        {
            Unknown = 0,
            Warning = 1,
            Critical = 2,
            Reserved = 3
        }

        // Command Status
        public readonly struct CommandStatusPage
        {
            public byte LastCommandReceived { get; }
            public byte SequenceNumber { get; }
            public CommandResult Status { get; }
            public uint ResponseData { get; }

            public CommandStatusPage(byte[] dataPage)
            {
                LastCommandReceived = dataPage[1];
                SequenceNumber = dataPage[2];
                Status = (CommandResult)dataPage[3];
                ResponseData = BitConverter.ToUInt32(dataPage, 4);
            }
        }
        // Manufacturer Info
        public readonly struct ManufacturerInfoPage
        {
            public byte HardwareRevision { get; }
            public ushort ManufacturerId { get; }
            public ushort ModelNumber { get; }

            public ManufacturerInfoPage(byte[] dataPage)
            {
                HardwareRevision = dataPage[3];
                ManufacturerId = BitConverter.ToUInt16(dataPage, 4);
                ModelNumber = BitConverter.ToUInt16(dataPage, 6);
            }
        }
        // Product Info
        public readonly struct ProductInfoPage
        {
            public Version SoftwareRevision { get; }
            public uint SerialNumber { get; }

            public ProductInfoPage(byte[] dataPage)
            {
                if (dataPage[2] != 0xFF)
                {
                    // supplemental SW revision is valid
                    SoftwareRevision = Version.Parse(((dataPage[3] * 100.0 + dataPage[2]) / 1000.0).ToString("N3"));
                }
                else
                {
                    // only main SW revision is present
                    SoftwareRevision = Version.Parse((dataPage[3] / 10.0).ToString("N3"));
                }
                SerialNumber = BitConverter.ToUInt32(dataPage, 4);
            }
        }
        // Battery Status
        public readonly struct BatteryStatusPage
        {
            public byte NumberOfBatteries { get; }
            public byte Identifier { get; }
            public TimeSpan CumulativeOperatingTime { get; }
            public BatteryStatus Status { get; }
            public double BatteryVoltage { get; }

            public BatteryStatusPage(byte[] dataPage)
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
        public readonly struct SubfieldDataPage
        {
            public enum SubPage
            {
                Temperature = 1,
                BarometricPressure,
                Humidity,
                WindSpeed,
                WindDirection,
                ChargingCycles,
                MinimumOperatingTemperature,
                MaximumOperatingTemperature,
                Invalid = 0xFF
            }

            public SubPage Subpage1 { get; }
            public double ComputedDataField1 { get; }
            public SubPage Subpage2 { get; }
            public double ComputedDataField2 { get; }

            public SubfieldDataPage(byte[] dataPage)
            {
                Subpage1 = (SubPage)dataPage[2];
                Subpage2 = (SubPage)dataPage[3];
                ComputedDataField1 = ParseSubfieldData(Subpage1, BitConverter.ToInt16(dataPage, 4));
                ComputedDataField2 = ParseSubfieldData(Subpage2, BitConverter.ToInt16(dataPage, 6));
            }

            private static double ParseSubfieldData(SubPage page, short value)
            {
                double retVal = 0;
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
        // Memory Level
        public readonly struct MemoryLevelPage
        {

            public double PercentUsed { get; }
            public double TotalSize { get; }
            public MemorySizeUnit TotalSizeUnit { get; }

            public MemoryLevelPage(byte[] dataPage)
            {
                PercentUsed = dataPage[4] * 0.5;
                TotalSize = BitConverter.ToUInt16(dataPage, 5) * 0.1;
                TotalSizeUnit = (MemorySizeUnit)(dataPage[7] & 0x83);
            }
        }
        // Error Description
        public readonly struct ErrorDescriptionPage
        {

            public byte SystemComponentIndex { get; }
            public ErrorLevel ErrorLevel { get; }
            public byte ProfileSpecificErrorCode { get; }
            public uint ManufacturerSpecificErrorCode { get; }

            public ErrorDescriptionPage(byte[] dataPage)
            {
                SystemComponentIndex = (byte)(dataPage[2] & 0x0F);
                ErrorLevel = (ErrorLevel)((dataPage[2] & 0xC0) >> 6);
                ProfileSpecificErrorCode = dataPage[3];
                ManufacturerSpecificErrorCode = BitConverter.ToUInt32(dataPage, 4);
            }
        }

        public CommandStatusPage CommandStatus { get; private set; }
        public ManufacturerInfoPage ManufacturerInfo { get; private set; }
        public ProductInfoPage ProductInfo { get; private set; }
        public BatteryStatusPage BatteryStatus { get; private set; }
        public DateTime TimeAndDate { get; private set; }
        public SubfieldDataPage SubfieldData { get; private set; }
        public MemoryLevelPage MemoryLevel { get; private set; }
        public ErrorDescriptionPage ErrorDescription { get; private set; }

        public void ParseCommonDataPage(byte[] dataPage)
        {
            switch ((CommonDataPage)dataPage[0])
            {
                case CommonDataPage.CommandStatus:
                    CommandStatus = new CommandStatusPage(dataPage);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CommandStatus"));
                    break;
                case CommonDataPage.GenericCommandPage:
                    break;
                case CommonDataPage.OpenChannelCommand:
                    break;
                case CommonDataPage.ModeSettingsPage:
                    break;
                case CommonDataPage.MultiComponentManufacturerInfo:
                    break;
                case CommonDataPage.MultiComponentProductInfo:
                    break;
                case CommonDataPage.ManufacturerInfo:
                    ManufacturerInfo = new ManufacturerInfoPage(dataPage);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ManufacturerInfo"));
                    break;
                case CommonDataPage.ProductInfo:
                    ProductInfo = new ProductInfoPage(dataPage);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProductInfo"));
                    break;
                case CommonDataPage.BatteryStatus:
                    BatteryStatus = new BatteryStatusPage(dataPage);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BatteryStatus"));
                    break;
                case CommonDataPage.TimeAndDate:
                    TimeAndDate = new DateTime(2000 + dataPage[7], dataPage[6], dataPage[5] & 0x1F, dataPage[4], dataPage[3], dataPage[2], DateTimeKind.Utc);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TimeAndDate"));
                    break;
                case CommonDataPage.SubfieldData:
                    SubfieldData = new SubfieldDataPage(dataPage);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SubfieldData"));
                    break;
                case CommonDataPage.MemoryLevel:
                    MemoryLevel = new MemoryLevelPage(dataPage);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MemoryLevel"));
                    break;
                case CommonDataPage.PairedDevices:
                    break;
                case CommonDataPage.ErrorDescription:
                    ErrorDescription = new ErrorDescriptionPage(dataPage);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ErrorDescription"));
                    break;
                default:
                    break;
            }
        }
    }
}
