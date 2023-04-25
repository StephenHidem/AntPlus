using System.ComponentModel;

namespace SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This class supports the treadmill fitness equipment type.
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public class Treadmill : INotifyPropertyChanged
    {
        private bool isFirstDataMessage = true;
        private byte prevPos;
        private byte prevNeg;

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Gets the cadence in strides per minute.</summary>
        public byte Cadence { get; private set; }
        /// <summary>Gets the accumulated negative vertical distance traveled in meters.</summary>
        public double NegVerticalDistance { get; private set; }
        /// <summary>Gets the accumulated positive vertical distance traveled in meters.</summary>
        public double PosVerticalDistance { get; private set; }

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public void Parse(byte[] dataPage)
        {
            Cadence = dataPage[4];
            if (isFirstDataMessage)
            {
                prevNeg = dataPage[5];
                prevPos = dataPage[6];
                isFirstDataMessage = false;
            }
            else
            {
                NegVerticalDistance += Utils.CalculateDelta(dataPage[5], ref prevNeg);
                PosVerticalDistance += Utils.CalculateDelta(dataPage[6], ref prevPos);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}
