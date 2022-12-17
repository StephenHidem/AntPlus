using System;
using System.Collections.Generic;
using System.Linq;

namespace AntPlus
{
    public enum CommonDataPage
    {
        AntFSClientBeacon = 0x43,
        AntFSCommandResponse = 0x44,
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

    public enum CommandStatus
    {
        Pass,
        Fail,
        NotSupported,
        Rejected,
        Pending
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

    public enum ConnectionState
    {
        Closed,
        Searching,
        Synchronized
    }

    public enum NetworkKey
    {
        Public = 0,
        Private = 1,
        AntPlusManaged = 2,
        AntFS = 3
    }

    public enum CommandType
    {
        Unknown,
        DataPage,
        AntFSSesion,
        DataPageFromSlave,
        DataPageSet
    }

    public class CommonDataPages
    {
        // ANT-FS Client Beacon
        public readonly struct AntFsClientBeaconPage
        {

            public byte StatusByte1 { get; }
            public byte StatusByte2 { get; }
            public byte AuthenticationType { get; }
            public uint DeviceDescriptorOrHostSerialNumber { get; }

            public AntFsClientBeaconPage(byte[] payload)
            {
                StatusByte1 = payload[1];
                StatusByte2 = payload[2];
                AuthenticationType = payload[3];
                DeviceDescriptorOrHostSerialNumber = BitConverter.ToUInt32(payload, 4);
            }
        }

        public readonly struct AntFsCommandResponsePage
        {
            public byte CommandResponseId { get; }
            public byte[] Parameters { get; }

            public AntFsCommandResponsePage(byte[] payload)
            {
                CommandResponseId = payload[1];
                Parameters = payload.Skip(2).ToArray();
            }
        }

        // Command Status
        public readonly struct CommandStatusPage
        {
            public byte LastCommandReceived { get; }
            public byte SequenceNumber { get; }
            public CommandStatus Status { get; }
            public uint ResponseData { get; }

            public CommandStatusPage(byte[] payload)
            {
                LastCommandReceived = payload[1];
                SequenceNumber = payload[2];
                Status = (CommandStatus)payload[3];
                ResponseData = BitConverter.ToUInt32(payload, 4);
            }
        }

        // Multiple components, both manufacture and product
        // TODO: REVIEW. THIS SHOULD LIKELY ENTAIL A LIST.
        public int NumberOfComponents { get; private set; }
        public int ComponentId { get; private set; }

        // Manufacturer Info
        public readonly struct ManufacturerInfoPage
        {
            public byte HardwareRevision { get; }
            public ushort ManufacturerId { get; }
            public ushort ModelNumber { get; }

            public ManufacturerInfoPage(byte[] payload)
            {
                HardwareRevision = payload[3];
                ManufacturerId = BitConverter.ToUInt16(payload, 4);
                ModelNumber = BitConverter.ToUInt16(payload, 6);
            }
        }

        // Product Info
        public readonly struct ProductInfoPage
        {
            public Version SoftwareRevision { get; }
            public uint SerialNumber { get; }

            public ProductInfoPage(byte[] payload)
            {
                if (payload[2] != 0xFF)
                {
                    // supplemental SW revision is valid
                    SoftwareRevision = Version.Parse(((payload[3] * 100.0 + payload[2]) / 1000.0).ToString("N3"));
                }
                else
                {
                    // only main SW revision is present
                    SoftwareRevision = Version.Parse((payload[3] / 10.0).ToString("N3"));
                }
                SerialNumber = BitConverter.ToUInt32(payload, 4);
            }
        }

        // Battery Status
        public readonly struct BatteryStatusPage
        {
            public byte NumberOfBatteries { get; }
            public byte Identifier { get; }
            public TimeSpan CumulativeOperatingTime { get; }
            public BatteryStatus BatteryStatus { get; }
            public double BatteryVoltage { get; }

            public BatteryStatusPage(byte[] payload)
            {
                if (payload[2] != 0xFF)
                {
                    NumberOfBatteries = (byte)(payload[2] & 0x0F);
                    Identifier = (byte)(payload[2] >> 4);
                }
                else
                {
                    NumberOfBatteries = 1; Identifier = 0;
                }
                CumulativeOperatingTime =
                    TimeSpan.FromSeconds((BitConverter.ToInt32(payload, 3) & 0x00FFFFFF) * (((payload[7] & 0x80) == 0x80) ? 2.0 : 16.0));
                BatteryVoltage = (payload[7] & 0x0F) + (payload[6] / 256.0);
                BatteryStatus = (BatteryStatus)((payload[7] & 0x70) >> 4);
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

            public SubfieldDataPage(byte[] payload)
            {
                Subpage1 = (SubPage)payload[2];
                Subpage2 = (SubPage)payload[3];
                ComputedDataField1 = ParseSubfieldData(Subpage1, BitConverter.ToInt16(payload, 4));
                ComputedDataField2 = ParseSubfieldData(Subpage2, BitConverter.ToInt16(payload, 6));
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
        public SubfieldDataPage SubfieldData { get; private set; }

        // Memory Level
        public readonly struct MemoryLevelPage
        {

            public double PercentUsed { get; }
            public double TotalSize { get; }
            public MemorySizeUnit TotalSizeUnit { get; }

            public MemoryLevelPage(byte[] payload)
            {
                PercentUsed = payload[4] * 0.5;
                TotalSize = BitConverter.ToUInt16(payload, 5) * 0.1;
                TotalSizeUnit = (MemorySizeUnit)(payload[7] & 0x83);
            }
        }
        public MemoryLevelPage MemoryLevel { get; private set; }

        // Error Description
        public readonly struct ErrorDescriptionPage
        {

            public byte SystemComponentIndex { get; }
            public ErrorLevel ErrorLevel { get; }
            public byte ProfileSpecificErrorCode { get; }
            public uint ManufacturerSpecificErrorCode { get; }

            public ErrorDescriptionPage(byte[] payload)
            {
                SystemComponentIndex = (byte)(payload[2] & 0x0F);
                ErrorLevel = (ErrorLevel)((payload[2] & 0xC0) >> 6);
                ProfileSpecificErrorCode = payload[3];
                ManufacturerSpecificErrorCode = BitConverter.ToUInt32(payload, 4);
            }
        }
        public ErrorDescriptionPage ErrorDescription { get; private set; }

        // Paired Devices
        public byte NumberOfConnectedDevices { get; private set; }
        public bool IsPaired { get; private set; }
        public ConnectionState ConnectionState { get; private set; }
        public NetworkKey NetworkKey { get; private set; }
        public readonly struct PairedDevice
        {
            public byte Index { get; }
            public uint PeripheralDeviceId { get; }
            public PairedDevice(byte index, uint deviceId) => (Index, PeripheralDeviceId) = (index, deviceId);
        }
        public List<PairedDevice> PairedDevices { get; private set; } = new List<PairedDevice>();

        public event EventHandler<AntFsClientBeaconPage> AntFsClientBeaconPageChanged;
        public event EventHandler<AntFsCommandResponsePage> AntFsCommandResponsePageChanged;
        public event EventHandler<CommandStatusPage> CommandStatusPageChanged;
        public event EventHandler<ManufacturerInfoPage> ManufacturerInfoPageChanged;
        public event EventHandler<ProductInfoPage> ProductInfoPageChanged;
        public event EventHandler<BatteryStatusPage> BatteryStatusPageChanged;
        public event EventHandler<DateTime> DateTimeChanged;
        public event EventHandler<SubfieldDataPage> SubfieldDataPageChanged;
        public event EventHandler<MemoryLevelPage> MemoryLevelPageChanged;
        public event EventHandler<ErrorDescriptionPage> ErrorDescriptionPageChanged;

        public void ParseCommonDataPage(byte[] payload)
        {
            switch ((CommonDataPage)payload[0])
            {
                case CommonDataPage.AntFSClientBeacon:
                    AntFsClientBeaconPageChanged?.Invoke(this, new AntFsClientBeaconPage(payload));
                    break;
                case CommonDataPage.AntFSCommandResponse:
                    AntFsCommandResponsePageChanged?.Invoke(this, new AntFsCommandResponsePage(payload));
                    break;
                case CommonDataPage.CommandStatus:
                    CommandStatusPageChanged?.Invoke(this, new CommandStatusPage(payload));
                    break;
                case CommonDataPage.GenericCommandPage:
                    break;
                case CommonDataPage.OpenChannelCommand:
                    break;
                case CommonDataPage.MultiComponentManufacturerInfo:
                    // TODO: REVIEW. THIS SHOULD LIKELY ENTAIL A LIST.
                    NumberOfComponents = payload[2] & 0x0F;
                    ComponentId = payload[2] >> 4;
                    goto case CommonDataPage.ManufacturerInfo;
                case CommonDataPage.ManufacturerInfo:
                    ManufacturerInfoPageChanged?.Invoke(this, new ManufacturerInfoPage(payload));
                    break;
                case CommonDataPage.MultiComponentProductInfo:
                    // TODO: REVIEW. THIS SHOULD LIKELY ENTAIL A LIST.
                    NumberOfComponents = payload[1] & 0x0F;
                    ComponentId = payload[1] >> 4;
                    goto case CommonDataPage.ProductInfo;
                case CommonDataPage.ProductInfo:
                    ProductInfoPageChanged?.Invoke(this, new ProductInfoPage(payload));
                    break;
                case CommonDataPage.BatteryStatus:
                    BatteryStatusPageChanged?.Invoke(this, new BatteryStatusPage(payload));
                    break;
                case CommonDataPage.TimeAndDate:
                    // note that day of week is ignored in payload since the DateTime struct can provide this
                    DateTimeChanged?.Invoke(this,
                        new DateTime(2000 + payload[7], payload[6], payload[5] & 0x1F, payload[4], payload[3], payload[2], DateTimeKind.Utc));
                    break;
                case CommonDataPage.SubfieldData:
                    SubfieldData = new SubfieldDataPage(payload);
                    SubfieldDataPageChanged?.Invoke(this, SubfieldData);
                    break;
                case CommonDataPage.MemoryLevel:
                    MemoryLevel = new MemoryLevelPage(payload);
                    MemoryLevelPageChanged?.Invoke(this, MemoryLevel);
                    break;
                case CommonDataPage.PairedDevices:
                    NumberOfConnectedDevices = payload[2];
                    IsPaired = (payload[3] & 0x80) == 0x80;
                    ConnectionState = (ConnectionState)((payload[3] & 0x38) >> 3);
                    NetworkKey = (NetworkKey)(payload[3] & 0x07);
                    // guard against adding the same device index
                    if (PairedDevices.Count == 0 || !PairedDevices.Exists(item => item.Index == payload[1]))
                    {
                        PairedDevices.Add(new PairedDevice(payload[1], BitConverter.ToUInt32(payload, 4)));
                    }
                    break;
                case CommonDataPage.ErrorDescription:
                    ErrorDescription = new ErrorDescriptionPage(payload);
                    ErrorDescriptionPageChanged?.Invoke(this, ErrorDescription);
                    break;
            }
        }
    }
}
