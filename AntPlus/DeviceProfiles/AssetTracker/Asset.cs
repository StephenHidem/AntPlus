using System;
using System.ComponentModel;
using System.Text;

namespace AntPlus.DeviceProfiles.AssetTracker
{
    /// <summary>
    /// This class represents an asset.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class Asset : INotifyPropertyChanged
    {
        private bool gotId;
        private bool gotLoc1 = false;
        private int lat;

        public enum AssetType
        {
            AssetTracker,
            Dog
        }

        public enum AssetSituation
        {
            Sitting,
            Moving,
            Pointed,
            Treed,
            Unknown,
            Undefined = 0xFF
        }

        [Flags]
        public enum AssetStatus
        {
            Good = 0x00,
            LowBattery = 0x08,
            GPSLost = 0x10,
            CommunicationLost = 0x20,
            RemoveAsset = 0x40,
        }

        public int Index { get; set; }
        public string Name { get; set; }
        public AssetType Type { get; set; }
        public byte Color { get; set; }
        /// <summary>Gets the distance.</summary>
        /// <value>The distance to the asset in meters.</value>
        public ushort Distance { get; private set; }
        /// <summary>Gets the bearing.</summary>
        /// <value>The bearing of the asset in degrees.</value>
        public double Bearing { get; private set; }
        public AssetStatus Status { get; private set; }
        public AssetSituation Situation { get; private set; }
        /// <summary>Gets the latitude.</summary>
        /// <value>The latitude of the asset in degress.</value>
        public double Latitude { get; private set; }
        /// <summary>Gets the longitude.</summary>
        /// <value>The longitude of the asset in degrees.</value>
        public double Longitude { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

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
            Bearing = 360.0 * data[4] / 256.0;
            Situation = (AssetSituation)(data[5] & 0x07);
            Status = (AssetStatus)(data[5] & 0x78);
            lat = BitConverter.ToUInt16(data, 6);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Distance"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Bearing"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Situation"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
            gotLoc1 = true;
        }
        public void ParseLocation2(byte[] data)
        {
            if (gotLoc1)
            {
                lat += BitConverter.ToUInt16(data, 2) << 16;
                Latitude = 180.0 * lat / Math.Pow(2, 31);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Latitude"));
                gotLoc1 = false;
            }
            Longitude = 180.0 * BitConverter.ToInt32(data, 4) / Math.Pow(2, 31);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Longitude"));
        }
    }
}
