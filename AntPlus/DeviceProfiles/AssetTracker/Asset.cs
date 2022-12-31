using System;
using System.Text;

namespace AntPlus.DeviceProfiles.AssetTracker
{
    public class Asset
    {
        private bool gotId;

        public enum AssetType
        {
            AssetTracker,
            Dog
        }

        public enum Situation
        {
            Sitting,
            Moving,
            Pointed,
            Treed,
            Unknown,
            Undefined = 0xFF
        }

        public readonly struct AssetStatus
        {
            private readonly byte stat;

            public Situation Situation => (Situation)(stat & 0x07);
            public bool LowBattery => (stat & 0x08) != 0;
            public bool GPSLost => (stat & 0x10) != 0;
            public bool CommunicationLost => (stat & 0x20) != 0;
            public bool RemoveAsset => (stat & 0x40) != 0;

            internal AssetStatus(byte status)
            {
                stat = status;
            }
        }

        public int Index { get; set; }
        public string Name { get; set; }
        public AssetType Type { get; set; }
        public byte Color { get; set; }
        public ushort Distance { get; private set; }
        public byte Bearing { get; private set; }
        public AssetStatus Status { get; private set; }
        public int Latitude { get; private set; }
        public int Longitude { get; private set; }

        public event EventHandler AssetChanged;

        public Asset(byte[] data)
        {
            Index = data[1] & 0x1F;
            Color = data[2];
            Name = Encoding.UTF8.GetString(data, 3, 5).TrimEnd((char)0);
        }

        public void ParseIdPage2(byte[] data)
        {
            if (gotId == false)
            {
                Type = (AssetType)data[2];
                Name += Encoding.UTF8.GetString(data, 3, 5).TrimEnd((char)0);
                gotId = true;
            }
        }

        public void ParseLocation1(byte[] data)
        {
            Distance = BitConverter.ToUInt16(data, 2);
            Bearing = data[4];
            Status = new AssetStatus(data[5]);
            Latitude = BitConverter.ToUInt16(data, 6);
        }
        public void ParseLocation2(byte[] data)
        {
            Latitude += BitConverter.ToUInt16(data, 2) << 16;
            Longitude = BitConverter.ToInt32(data, 4);
            AssetChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
