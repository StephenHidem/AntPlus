using System;
using System.ComponentModel;

namespace AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This class supports Nordic skiers fitness equipment type.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class NordicSkier : INotifyPropertyChanged
    {
        private bool isFirstDataMessage = true;
        private byte prevStride;

        /// <summary>Gets the stride count.</summary>
        public int StrideCount { get; private set; }
        /// <summary>Gets the cadence in strides per minute.</summary>
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
                prevStride = dataPage[3];
            }
            else
            {
                StrideCount += Utils.CalculateDelta(dataPage[3], ref prevStride);
            }
            Cadence = dataPage[4];
            InstantaneousPower = BitConverter.ToUInt16(dataPage, 5);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}
