using System;
using System.ComponentModel;

namespace SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This class supports the elliptical fitness equipment type.
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public class Elliptical : INotifyPropertyChanged
    {
        private bool isFirstDataMessage = true;
        private byte prevPos;
        private byte prevStride;

        /// <summary>Elliptical specific capabilities.</summary>
        [Flags]
        public enum CapabilityFlags
        {
            /// <summary>Transmits stride count.</summary>
            TxStrideCount = 0x01,
            /// <summary>Transmits positive vertical distance.</summary>
            TxPosVertDistance = 0x02
        }

        /// <summary>Gets the stride count. This is an accumulated value.</summary>
        public int StrideCount { get; private set; }
        /// <summary>Gets the cadence in strides per minute.</summary>
        public byte Cadence { get; private set; }
        /// <summary>Gets the positive vertical distance in meters.</summary>
        public double PosVerticalDistance { get; private set; }
        /// <summary>Gets the instantaneous power in watts.</summary>
        public int InstantaneousPower { get; private set; }
        /// <summary>Gets the elliptical specific capabilities.</summary>
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
                prevPos = dataPage[2];
                prevStride = dataPage[3];
            }
            else
            {
                PosVerticalDistance += Utils.CalculateDelta(dataPage[2], ref prevPos);
                StrideCount += Utils.CalculateDelta(dataPage[3], ref prevStride);
            }
            Cadence = dataPage[4];
            InstantaneousPower = BitConverter.ToUInt16(dataPage, 5);
            Capabilities = (CapabilityFlags)(dataPage[7] & 0x03);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}
