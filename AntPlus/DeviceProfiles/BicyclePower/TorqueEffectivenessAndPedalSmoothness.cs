using System.ComponentModel;

namespace AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// This class supports torque effectiveness and pedal smoothness messages.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class TorqueEffectivenessAndPedalSmoothness : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public double LeftTorqueEffectivenes { get; private set; }
        public double RightTorqueEffectivenes { get; private set; }
        public double LeftPedalSmoothness { get; private set; }
        public double RightPedalSmoothness { get; private set; }
        public bool CombinedPedalSmoothness { get; private set; }

        /// <summary>
        /// Parses the torque effectiveness and pedal smoothness data page.
        /// Note that if the right and left pedal smootness is combined, the right and left values will be the same.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public void Parse(byte[] dataPage)
        {
            if (dataPage[2] != 0xFF) { LeftTorqueEffectivenes = dataPage[2] * 0.5; }
            else { LeftTorqueEffectivenes = 0; }

            if (dataPage[3] != 0xFF) { RightTorqueEffectivenes = dataPage[3] * 0.5; }
            else { RightTorqueEffectivenes = 0; }

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
