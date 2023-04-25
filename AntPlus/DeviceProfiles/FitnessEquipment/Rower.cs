using System;
using System.ComponentModel;

namespace SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This class supports the rower fitness equipment type.
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public class Rower : INotifyPropertyChanged
    {
        private bool isFirstDataMessage = true;
        private byte prevStroke;

        /// <summary>Gets the accumulated stroke count.</summary>
        public int StrokeCount { get; private set; }
        /// <summary>Gets the cadence in strokes per minute.</summary>
        public byte Cadence { get; private set; }
        /// <summary>Gets the instantaneous power in watts.</summary>
        public int InstantaneousPower { get; private set; }

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public void Parse(byte[] dataPage)
        {
            if (isFirstDataMessage)
            {
                isFirstDataMessage = false;
                prevStroke = dataPage[3];
            }
            else
            {
                StrokeCount += Utils.CalculateDelta(dataPage[3], ref prevStroke);
            }
            Cadence = dataPage[4];
            InstantaneousPower = BitConverter.ToUInt16(dataPage, 5);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}
