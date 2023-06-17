using System;
using System.ComponentModel;

namespace SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This class supports Nordic skiers fitness equipment type.
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public class NordicSkier : INotifyPropertyChanged
    {
        private bool isFirstDataMessage = true;
        private byte prevStride;

        /// <summary>Nordic skier specific capabilities.</summary>
        public enum CapabilityFlags
        {
            /// <summary>No supported capabilities.</summary>
            None,
            /// <summary>Transmits accumulated stride count.</summary>
            TxStrideCount = 0x01,
        }

        /// <summary>Gets the stride count.</summary>
        public int StrideCount { get; private set; }
        /// <summary>Gets the cadence in strides per minute.</summary>
        public byte Cadence { get; private set; }
        /// <summary>Gets the instantaneous power in watts.</summary>
        public int InstantaneousPower { get; private set; }
        /// <summary>Gets the Nordic skier specific capabilities.</summary>
        /// <value>The capabilities.</value>
        public CapabilityFlags Capabilities { get; private set; }

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
            Capabilities = (CapabilityFlags)(dataPage[7] & 0x01);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}
