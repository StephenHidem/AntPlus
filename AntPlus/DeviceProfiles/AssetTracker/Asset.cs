using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Text;

namespace SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker
{
    /// <summary>
    /// This class represents an asset.
    /// </summary>
    public partial class Asset : ObservableObject
    {
        private bool gotIdPage1, gotIdPage2;
        private string upperName = string.Empty, lowerName = string.Empty;
        private bool gotLocationPage1, gotLocationPage2;
        private ushort lowerLatitude, upperLatitude;

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
            Pointing,
            /// <summary>Treed</summary>
            Treed,
            /// <summary>Unknown</summary>
            Unknown,
            /// <summary>Undefined</summary>
            Undefined = 7
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
            /// <summary>Lost communication with the transmitter node</summary>
            CommunicationLost = 0x20,
            /// <summary>Remove asset</summary>
            RemoveAsset = 0x40,
        }

        /// <summary>Gets the index of the asset.</summary>
        [ObservableProperty]
        private int index;
        /// <summary>Gets the name of the asset.</summary>
        [ObservableProperty]
        private string name = string.Empty;
        /// <summary>Gets the type of the asset.</summary>
        [ObservableProperty]
        private AssetType type;
        /// <summary>Gets the color of the asset. This is an 8 bit RGB value.</summary>
        [ObservableProperty]
        private byte color;
        /// <summary>Gets the distance.</summary>
        [ObservableProperty]
        private ushort distance;
        /// <summary>Gets the bearing.</summary>
        [ObservableProperty]
        private double bearing;
        /// <summary>Gets the status of the asset.</summary>
        [ObservableProperty]
        private AssetStatus status;
        /// <summary>Gets the situation of the asset. This typically applies to dogs.</summary>
        [ObservableProperty]
        private AssetSituation situation;
        /// <summary>Gets the latitude.</summary>
        [ObservableProperty]
        private double latitude;
        /// <summary>Gets the longitude.</summary>
        [ObservableProperty]
        private double longitude;

        /// <summary>
        /// Initializes a new instance of the <see cref="Asset"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public Asset(byte[] data)
        {
            Index = data[1] & 0x1F;
        }

        /// <summary>
        /// Parses the identifier page1. This provides the asset color and the first part of the name.
        /// </summary>
        /// <param name="data">The data.</param>
        internal void ParseIdPage1(byte[] data)
        {
            Color = data[2];
            if (!gotIdPage1)
            {
                upperName = Encoding.UTF8.GetString(data, 3, 5).TrimEnd((char)0);
                gotIdPage1 = true;
            }
            if (gotIdPage1 && gotIdPage2)
            {
                Name = upperName + lowerName;
                gotIdPage1 = gotIdPage2 = false;
            }
        }

        /// <summary>
        /// Parses the identifier page2. This provides the asset type and the rest of the name.
        /// </summary>
        /// <param name="data">The data.</param>
        internal void ParseIdPage2(byte[] data)
        {
            Type = (AssetType)data[2];
            if (!gotIdPage2)
            {
                lowerName = Encoding.UTF8.GetString(data, 3, 5).TrimEnd((char)0);
                gotIdPage2 = true;
            }
            if (gotIdPage1 && gotIdPage2)
            {
                Name = upperName + lowerName;
                gotIdPage1 = gotIdPage2 = false;
            }
        }

        /// <summary>
        /// Parses location page 1. This provides the distance, bearing, situation, status, and
        /// part of the latitude of the asset sensor being tracked.
        /// </summary>
        /// <param name="data">The data.</param>
        internal void ParseLocation1(byte[] data)
        {
            Distance = BitConverter.ToUInt16(data, 2);
            Bearing = 360.0 * data[4] / 256.0;
            Situation = (AssetSituation)(data[5] & 0x07);
            Status = (AssetStatus)(data[5] & 0x78);
            if (!gotLocationPage1)
            {
                lowerLatitude = BitConverter.ToUInt16(data, 6);
                gotLocationPage1 = true;
            }
            if (gotLocationPage1 && gotLocationPage2)
            {
                Latitude = Utils.SemicirclesToDegrees((upperLatitude << 16) | lowerLatitude);
                gotLocationPage1 = gotLocationPage2 = false;
            }
        }

        /// <summary>
        /// Parses location page 2. This provides the latitude and longitude of the asset sensor being tracked.
        /// </summary>
        /// <param name="data">The data.</param>
        internal void ParseLocation2(byte[] data)
        {
            if (!gotLocationPage2)
            {
                upperLatitude = BitConverter.ToUInt16(data, 2);
                gotLocationPage2 = true;
            }
            Longitude = Utils.SemicirclesToDegrees(BitConverter.ToInt32(data, 4));
            if (gotLocationPage1 && gotLocationPage2)
            {
                Latitude = Utils.SemicirclesToDegrees((upperLatitude << 16) | lowerLatitude);
                gotLocationPage1 = gotLocationPage2 = false;
            }
        }
    }
}
