using System;
using System.Collections.Generic;

namespace AntPlus
{
    public enum CommonDataPageType
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

    public enum SubfieldDataPage
    {
        Temperature = 1,
        BarometricPressure,
        Humidity,
        WindSpeed,
        WindDirection,
        ChargingCycles,
        MinimumOperatingTemperature,
        MaximumOperatingTemperature
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
        public byte StatusByte1 { get; private set; }
        public byte StatusByte2 { get; private set; }
        public byte AuthenticationType { get; private set; }
        public uint DeviceDescriptorOrHostSerialNumber { get; private set; }

        // Command Status
        public byte LastCommandReceived { get; private set; }
        public byte SequenceNumber { get; private set; } = 0;
        public CommandStatus CommandStatus { get; private set; }
        public uint ResponseData { get; private set; }

        // Multiple components, both manufacture and product
        // TODO: REVIEW. THIS SHOULD LIKELY ENTAIL A LIST.
        public int NumberOfComponents { get; private set; }
        public int ComponentId { get; private set; }

        // Manufacturer Info
        public byte HardwareRevision { get; private set; } = 0;
        public ushort ManufactureId { get; private set; } = 0;
        public ushort ModelNumber { get; private set; } = 0;

        // Product Info
        public Version SoftwareRevision { get; private set; }
        public uint SerialNumber { get; private set; } = 0xFFFFFFFF;

        // Battery Status
        public byte NumberOfBatteries { get; private set; }
        public byte Identifier { get; private set; }
        public TimeSpan CumulativeOperatingTime { get; private set; }
        public BatteryStatus BatteryStatus { get; private set; }
        public double BatteryVoltage { get; private set; }

        // Time and Date
        public DateTime TimeAndDate { get; private set; }

        // Memory Level
        public double PercentUsed { get; private set; }
        public double TotalSize { get; private set; }
        public MemorySizeUnit TotalSizeUnit { get; private set; }

        // Error Description
        public byte SystemComponentIndex { get; private set; }
        public ErrorLevel ErrorLevel { get; private set; }
        public byte ProfileSpecificErrorCode { get; private set; }
        public uint ManufacturerSpecificErrorCode { get; private set; }

        // Paired Devices
        public byte NumberOfConnectedDevices { get; private set; }
        public bool IsPaired { get; private set; }
        public ConnectionState ConnectionState { get; private set; }
        public NetworkKey NetworkKey { get; private set; }
        public struct PairedDevice
        {
            public byte Index { get; }
            public uint PeripheralDeviceId { get; }
            public PairedDevice(byte index, uint deviceId) => (Index, PeripheralDeviceId) = (index, deviceId);
        }
        public List<PairedDevice> PairedDevices { get; private set; } = new List<PairedDevice>();

        public void ParseCommonDataPage(byte[] payload)
        {
            CommonDataPageType pageType = (CommonDataPageType)payload[0];
            switch (pageType)
            {
                case CommonDataPageType.AntFSClientBeacon:
                    break;
                case CommonDataPageType.AntFSCommandResponse:
                    break;
                case CommonDataPageType.CommandStatus:
                    LastCommandReceived = payload[1];
                    SequenceNumber = payload[2];
                    CommandStatus = (CommandStatus)payload[3];
                    ResponseData = BitConverter.ToUInt32(payload, 4);
                    break;
                case CommonDataPageType.GenericCommandPage:
                    break;
                case CommonDataPageType.OpenChannelCommand:
                    break;
                case CommonDataPageType.ModeSettingsPage:
                    break;
                case CommonDataPageType.MultiComponentManufacturerInfo:
                    // TODO: REVIEW. THIS SHOULD LIKELY ENTAIL A LIST.
                    NumberOfComponents = payload[2] & 0x0F;
                    ComponentId = payload[2] >> 4;
                    goto case CommonDataPageType.ManufacturerInfo;
                case CommonDataPageType.ManufacturerInfo:
                    HardwareRevision = payload[3];
                    ManufactureId = BitConverter.ToUInt16(payload, 4);
                    ModelNumber = BitConverter.ToUInt16(payload, 6);
                    break;
                case CommonDataPageType.MultiComponentProductInfo:
                    // TODO: REVIEW. THIS SHOULD LIKELY ENTAIL A LIST.
                    NumberOfComponents = payload[1] & 0x0F;
                    ComponentId = payload[1] >> 4;
                    goto case CommonDataPageType.ProductInfo;
                case CommonDataPageType.ProductInfo:
                    if (payload[2] != 0xFF)
                    {
                        // supplemental SW revision is valid
                        SoftwareRevision = Version.Parse((((double)payload[3] * 100 + payload[2]) / 1000).ToString("N3"));
                    }
                    else
                    {
                        // only main SW revision is present
                        SoftwareRevision = Version.Parse(((double)payload[3] / 10).ToString("N3"));
                    }
                    SerialNumber = BitConverter.ToUInt32(payload, 4);
                    break;
                case CommonDataPageType.BatteryStatus:
                    if (payload[2] != 0xFF)
                    {
                        NumberOfBatteries = (byte)(payload[2] & 0x0F);
                        Identifier = (byte)(payload[2] >> 4);
                    }
                    CumulativeOperatingTime =
                        TimeSpan.FromSeconds((BitConverter.ToInt32(payload, 3) & 0x00FFFFFF) * (((payload[7] & 0x80) == 0x80) ? 2.0 : 16.0));
                    BatteryVoltage = (payload[7] & 0x0F) + (payload[6] / 256.0);
                    BatteryStatus = (BatteryStatus)((payload[7] & 0x70) >> 4);
                    break;
                case CommonDataPageType.TimeAndDate:
                    // note that day of week is ignored in payload since the DateTime struct can provide this
                    TimeAndDate = new DateTime(2000 + payload[7], payload[6], payload[5] & 0x1F, payload[4], payload[3], payload[2], DateTimeKind.Utc);
                    break;
                case CommonDataPageType.SubfieldData:
                    ParseSubfieldData((SubfieldDataPage)payload[2], BitConverter.ToInt16(payload, 4));
                    ParseSubfieldData((SubfieldDataPage)payload[3], BitConverter.ToInt16(payload, 6));
                    break;
                case CommonDataPageType.MemoryLevel:
                    PercentUsed = payload[4] * 0.5;
                    TotalSize = BitConverter.ToUInt16(payload, 5) * 0.1;
                    TotalSizeUnit = (MemorySizeUnit)(payload[7] & 0x83);
                    break;
                case CommonDataPageType.PairedDevices:
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
                case CommonDataPageType.ErrorDescription:
                    SystemComponentIndex = (byte)(payload[2] & 0x0F);
                    ErrorLevel = (ErrorLevel)((payload[2] & 0xC0) >> 6);
                    ProfileSpecificErrorCode = payload[3];
                    ManufacturerSpecificErrorCode = BitConverter.ToUInt32(payload, 4);
                    break;
            }
        }

        public double Temperature { get; set; }
        public double BarometricPressue { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
        public double WindDirection { get; set; }
        public ushort ChargingCycles { get; set; }
        public double MinimumOperatingTemperature { get; set; }
        public double MaximumOperatingTemperature { get; set; }

        private void ParseSubfieldData(SubfieldDataPage page, short value)
        {
            switch (page)
            {
                case SubfieldDataPage.Temperature:
                    Temperature = value * 0.01;
                    break;
                case SubfieldDataPage.BarometricPressure:
                    BarometricPressue = (ushort)value * 0.01;
                    break;
                case SubfieldDataPage.Humidity:
                    Humidity = value / 100.0;
                    break;
                case SubfieldDataPage.WindSpeed:
                    WindSpeed = (ushort)value * 0.01;
                    break;
                case SubfieldDataPage.WindDirection:
                    WindDirection = value / 20.0;
                    break;
                case SubfieldDataPage.ChargingCycles:
                    ChargingCycles = (ushort)value;
                    break;
                case SubfieldDataPage.MinimumOperatingTemperature:
                    MinimumOperatingTemperature = value / 100.0;
                    break;
                case SubfieldDataPage.MaximumOperatingTemperature:
                    MaximumOperatingTemperature = value / 100.0;
                    break;
                default:
                    break;
            }
        }
    }
}
