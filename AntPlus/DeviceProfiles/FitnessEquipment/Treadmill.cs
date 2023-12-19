using System;
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

        /// <summary>Treadmill specific capabilities.</summary>
        [Flags]
        public enum CapabilityFlags
        {
            /// <summary>Positive and negative vertical distance is not transmitted.</summary>
            None = 0x00,
            /// <summary>Transmits positive vertical distance.</summary>
            TxPosVertDistance = 0x01,
            /// <summary>Transmits negative vertical distance.</summary>
            TxNegVertDistance = 0x02
        }

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Gets the cadence in strides per minute.</summary>
        public byte Cadence { get; private set; }
        /// <summary>Gets the accumulated negative vertical distance traveled in meters. Note this is a negative value.</summary>
        public double NegVerticalDistance { get; private set; }
        /// <summary>Gets the accumulated positive vertical distance traveled in meters.</summary>
        public double PosVerticalDistance { get; private set; }
        /// <summary>Gets the treadmill specific capabilities.</summary>
        /// <value>The capabilities.</value>
        public CapabilityFlags Capabilities { get; private set; }

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public void Parse(byte[] dataPage)
        {
            Cadence = dataPage[4];
            Capabilities = (CapabilityFlags)(dataPage[7] & 0x03);
            if (isFirstDataMessage)
            {
                prevNeg = dataPage[5];
                prevPos = dataPage[6];
                isFirstDataMessage = false;
            }
            else
            {
                NegVerticalDistance += Utils.CalculateDelta(dataPage[5], ref prevNeg) / -10.0;
                PosVerticalDistance += Utils.CalculateDelta(dataPage[6], ref prevPos) / 10.0;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}
