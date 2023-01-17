using System;
using System.ComponentModel;

namespace AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This class supports the climber fitness equipment type.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class Climber : INotifyPropertyChanged
    {
        private bool isFirstDataMessage = true;
        private byte prevStride;

        public int StrideCycles { get; private set; }
        public byte Cadence { get; private set; }
        public int InstantaneousPower { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}
