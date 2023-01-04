using System.ComponentModel;

namespace AntPlus.DeviceProfiles.FitnessEquipment
{
    public class Treadmill : INotifyPropertyChanged
    {
        private bool isFirstDataMessage = true;
        private byte prevPos;
        private byte prevNeg;

        public byte Cadence { get; private set; }
        public double NegVerticalDistance { get; private set; }
        public double PosVerticalDistance { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

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
