using System;
using System.ComponentModel;

namespace SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This class supports the stationary bike fitness equipment type.
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public class TrainerStationaryBike : INotifyPropertyChanged
    {
        private bool isFirstDataMessage = true;
        private byte lastEventCount;
        private ushort lastPower;

        /// <summary>The trainer status bit field is used to indicate whether the trainer requires calibration and/or configuration data to be sent.</summary>
        [Flags]
        public enum TrainerStatusField
        {
            /// <summary>Complete or not required.</summary>
            None = 0x00,
            /// <summary>Bike power calibration required.</summary>
            BikePowerCalRequired = 0x10,
            /// <summary>Resistance calibration required.</summary>
            ResistanceCalRequired = 0x20,
            /// <summary>User configuration required.</summary>
            UserConfigRequired = 0x40,
        }

        /// <summary>Used by trainers operating in target power mode.</summary>
        public enum TargetPowerLimit
        {
            /// <summary>At target power or no target power set.</summary>
            AtTargetPower = 0,
            /// <summary>Cycling speed is too low to achieve target power.</summary>
            TooLow = 1,
            /// <summary>Cycling speed is too high to achieve target power.</summary>
            TooHigh = 2,
            /// <summary>Target power limit is undetermined.</summary>
            Undetermined = 3,
        }

        /// <summary>Gets the average power in watts.</summary>
        /// <value>The average power.</value>
        public double AveragePower { get; private set; }
        /// <summary>Gets the instantaneous pedaling cadence in revolutions per minute.</summary>
        /// <value>The instantaneous cadence.</value>
        public byte InstantaneousCadence { get; private set; }
        /// <summary>Gets the instantaneous power in watts.</summary>
        /// <value>The instantaneous power.</value>
        public int InstantaneousPower { get; private set; }
        /// <summary>Gets the trainer status.</summary>
        public TrainerStatusField TrainerStatus { get; private set; }
        /// <summary>Gets the trainer target power.</summary>
        public TargetPowerLimit TargetPower { get; private set; }
        /// <summary>Gets the trainer torque.</summary>
        public TrainerTorque TrainerTorque { get; private set; } = new TrainerTorque();

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
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
                int deltaEventCount = Utils.CalculateDelta(dataPage[1], ref lastEventCount);
                int deltaPower = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 3), ref lastPower);
                AveragePower = deltaPower / deltaEventCount;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}
