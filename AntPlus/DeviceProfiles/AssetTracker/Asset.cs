using System;
using System.ComponentModel;
using System.Text;

namespace SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker
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

        /// <summary>
        /// The asset being tracked.
        /// </summary>
        public enum AssetType
        {
            /// <summary>Asset tracker</summary>
            AssetTracker,
            /// <summary>Dog</summary>
            Dog
        }

        /// <summary>
        /// The asset situation. This typically applies to dogs.
        /// </summary>
        public enum AssetSituation
        {
            /// <summary>Sitting</summary>
            Sitting,
            /// <summary>Moving</summary>
            Moving,
            /// <summary>Pointing</summary>
            Pointed,
            /// <summary>Treed</summary>
            Treed,
            /// <summary>Unknown</summary>
            Unknown,
            /// <summary>Undefined</summary>
            Undefined = 0xFF
        }

        /// <summary>
        /// Status of the asset sensor being tracked.
        /// </summary>
        [Flags]
        public enum AssetStatus
        {
            /// <summary>Good</summary>
            Good = 0x00,
            /// <summary>Low battery</summary>
            LowBattery = 0x08,
            /// <summary>GPS lost track</summary>
            GPSLost = 0x10,
            /// <summary>Lost communication with GPS</summary>
            CommunicationLost = 0x20,
            /// <summary>Remove asset</summary>
            RemoveAsset = 0x40,
        }

        /// <summary>
        /// Gets the index of the asset.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; private set; }
        /// <summary>
        /// Gets the name of the asset.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }
        /// <summary>
        /// Gets the type of the asset.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public AssetType Type { get; private set; }
        /// <summary>
        /// Gets the color of the asset. This is an 8 bit RGB value.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public byte Color { get; private set; }
        /// <summary>Gets the distance.</summary>
        /// <value>The distance to the asset in meters.</value>
        public ushort Distance { get; private set; }
        /// <summary>Gets the bearing.</summary>
        /// <value>The bearing of the asset in degrees.</value>
        public double Bearing { get; private set; }
        /// <summary>
        /// Gets the status of the asset.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public AssetStatus Status { get; private set; }
        /// <summary>
        /// Gets the situation of the asset. This typically applies to dogs.
        /// </summary>
        /// <value>
        /// The situation.
        /// </value>
        public AssetSituation Situation { get; private set; }
        /// <summary>Gets the latitude.</summary>
        /// <value>The latitude of the asset in degrees.</value>
        public double Latitude { get; private set; }
        /// <summary>Gets the longitude.</summary>
        /// <value>The longitude of the asset in degrees.</value>
        public double Longitude { get; private set; }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="Asset"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public Asset(byte[] data)
        {
            Index = data[1] & 0x1F;
            Color = data[2];
            Name = Encoding.UTF8.GetString(data, 3, 5).TrimEnd((char)0);
        }

        /// <summary>
        /// Parses the identifier page2. This provides the asset type and the rest of the name.
        /// </summary>
        /// <param name="data">The data.</param>
        internal void ParseIdPage2(byte[] data)
        {
            if (gotId == false)
            {
                Type = (AssetType)data[2];
                Name += Encoding.UTF8.GetString(data, 3, 5).TrimEnd((char)0);
                gotId = true;
            }
        }

        /// <summary>
        /// Parses the location1 page. This provides the distance, bearing, situation, status, and
        /// part of the latitude of the asset sensor being tracked.
        /// </summary>
        /// <param name="data">The data.</param>
        internal void ParseLocation1(byte[] data)
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

        /// <summary>
        /// Parses the location2 page. This provides the latitude and longitude of the asset sensor being tracked.
        /// </summary>
        /// <param name="data">The data.</param>
        internal void ParseLocation2(byte[] data)
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
