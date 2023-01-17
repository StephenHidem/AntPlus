using System;
using System.ComponentModel;

namespace AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This class supports the stationary bike fitness equipment type.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class TrainerStationaryBike : INotifyPropertyChanged
    {
        private bool isFirstDataMessage = true;
        private byte lastEventCount;
        private int deltaEventCount;
        private ushort lastPower;
        private int deltaPower;

        [Flags]
        public enum TrainerStatusField
        {
            BikePowerCalRequired = 0x10,
            ResistanceCalRequired = 0x20,
            UserConfigRequired = 0x40,
        }

        public enum TargetPowerLimit
        {
            AtTargetPower = 0,
            TooLow = 1,
            TooHigh = 2,
            Undetermined = 3,
        }

        public double AveragePower { get; private set; }
        public byte InstantaneousCadence { get; private set; }
        public int InstantaneousPower { get; private set; }
        public TrainerStatusField TrainerStatus { get; private set; }
        public TargetPowerLimit TargetPower { get; private set; }
        public TrainerTorque TrainerTorque { get; private set; } = new TrainerTorque();

        public event PropertyChangedEventHandler PropertyChanged;

        public void Parse(byte[] dataPage)
        {
            InstantaneousCadence = dataPage[2];
            InstantaneousPower = BitConverter.ToUInt16(dataPage, 5) & 0x0FFF;
            TrainerStatus = (TrainerStatusField)(dataPage[6] & 0xF0);
            TargetPower = (TargetPowerLimit)(dataPage[7] & 0x0F);

            if (isFirstDataMessage)
            {
                // initialize if first data message
                isFirstDataMessage = false;
                lastEventCount = dataPage[1];
                lastPower = BitConverter.ToUInt16(dataPage, 3);
                return;
            }

            if (dataPage[1] != lastEventCount)
            {
                // handle new events
                deltaEventCount = Utils.CalculateDelta(dataPage[1], ref lastEventCount);
                deltaPower = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 3), ref lastPower);
                AveragePower = deltaPower / deltaEventCount;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }

        public void ParseTorque(byte[] dataPage)
        {
            TrainerTorque.Parse(dataPage);
        }
    }
}
