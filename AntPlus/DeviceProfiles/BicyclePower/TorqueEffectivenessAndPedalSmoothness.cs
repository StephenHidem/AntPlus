using System.ComponentModel;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// This class supports torque effectiveness and pedal smoothness messages.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class TorqueEffectivenessAndPedalSmoothness : INotifyPropertyChanged
    {
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Gets the left leg torque effectiveness as a percentage.</summary>
        public double LeftTorqueEffectiveness { get; private set; }
        /// <summary>Gets the right leg torque effectiveness as a percentage.</summary>
        public double RightTorqueEffectiveness { get; private set; }
        /// <summary>Gets the left pedal smoothness as a percentage.</summary>
        public double LeftPedalSmoothness { get; private set; }
        /// <summary>Gets the right pedal smoothness as a percentage.</summary>
        public double RightPedalSmoothness { get; private set; }
        /// <summary>Set to true if left and right pedal smoothness is combined.</summary>
        public bool CombinedPedalSmoothness { get; private set; }

        /// <summary>
        /// Parses the torque effectiveness and pedal smoothness data page.
        /// Note that if the right and left pedal smootness is combined, the right and left values will be the same.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public void Parse(byte[] dataPage)
        {
            if (dataPage[2] != 0xFF) { LeftTorqueEffectiveness = dataPage[2] * 0.5; }
            else { LeftTorqueEffectiveness = 0; }

            if (dataPage[3] != 0xFF) { RightTorqueEffectiveness = dataPage[3] * 0.5; }
            else { RightTorqueEffectiveness = 0; }

            if (dataPage[4] != 0xFF) { LeftPedalSmoothness = dataPage[4] * 0.5; }
            else { LeftPedalSmoothness = 0; }

            if (dataPage[5] != 0xFF)
            {
                if (dataPage[5] != 0xFE)
                {
                    CombinedPedalSmoothness = false;
                    RightPedalSmoothness = dataPage[5] * 0.5;
                }
                else
                {
                    CombinedPedalSmoothness = true;
                    RightPedalSmoothness = dataPage[4] * 0.5;
                }
            }
            else
            {
                CombinedPedalSmoothness = false;
                RightPedalSmoothness = 0;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}
