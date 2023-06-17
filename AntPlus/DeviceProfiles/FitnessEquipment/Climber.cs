using System;
using System.ComponentModel;

namespace SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This class supports the climber fitness equipment type.
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public class Climber : INotifyPropertyChanged
    {
        private bool isFirstDataMessage = true;
        private byte prevStride;

        /// <summary>Climber specific capabilities.</summary>
        public enum CapabilityFlags
        {
            /// <summary>Accumulated strides are not transmitted.</summary>
            None = 0x00,
            /// <summary>Transmits accumulated strides.</summary>
            TxStrides = 0x01,
        }

        /// <summary>Gets the stride cycles. Accumulated value of the complete number of stride cycles (i.e. number of steps climbed/2)</summary>
        public int StrideCycles { get; private set; }
        /// <summary>Gets the cadence in stride cycles per minuter.</summary>
        public byte Cadence { get; private set; }
        /// <summary>Gets the instantaneous power in watts.</summary>
        public int InstantaneousPower { get; private set; }
        /// <summary>Gets the climber specific capabilities.</summary>
        /// <value>The capabilities.</value>
        public CapabilityFlags Capabilities { get; private set; }

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Parses the specified data page.</summary>
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
                StrideCycles += Utils.CalculateDelta(dataPage[3], ref prevStride);
            }
            Cadence = dataPage[4];
            InstantaneousPower = BitConverter.ToUInt16(dataPage, 5);
            Capabilities = (CapabilityFlags)(dataPage[7] & 0x01);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}
