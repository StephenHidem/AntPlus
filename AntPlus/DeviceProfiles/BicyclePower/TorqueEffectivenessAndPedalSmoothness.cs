using CommunityToolkit.Mvvm.ComponentModel;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// This class supports torque effectiveness and pedal smoothness messages.
    /// </summary>
    public partial class TorqueEffectivenessAndPedalSmoothness : ObservableObject
    {
        /// <summary>Gets the left leg torque effectiveness as a percentage.</summary>
        [ObservableProperty]
        private double leftTorqueEffectiveness;
        /// <summary>Gets the right leg torque effectiveness as a percentage.</summary>
        [ObservableProperty]
        private double rightTorqueEffectiveness;
        /// <summary>Gets the left pedal smoothness as a percentage.</summary>
        [ObservableProperty]
        private double leftPedalSmoothness;
        /// <summary>Gets the right pedal smoothness as a percentage.</summary>
        [ObservableProperty]
        private double rightPedalSmoothness;
        /// <summary>Set to true if left and right pedal smoothness is combined.</summary>
        [ObservableProperty]
        private bool combinedPedalSmoothness;

        /// <summary>
        /// Parses the torque effectiveness and pedal smoothness data page.
        /// Note that if the right and left pedal smoothness is combined, the right and left values will be the same.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public void Parse(byte[] dataPage)
        {
            if (dataPage[2] != 0xFF) { LeftTorqueEffectiveness = dataPage[2] * 0.5; }
            else { LeftTorqueEffectiveness = double.NaN; }

            if (dataPage[3] != 0xFF) { RightTorqueEffectiveness = dataPage[3] * 0.5; }
            else { RightTorqueEffectiveness = double.NaN; }

            if (dataPage[4] != 0xFF) { LeftPedalSmoothness = dataPage[4] * 0.5; }
            else { LeftPedalSmoothness = double.NaN; }

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
                RightPedalSmoothness = double.NaN;
            }
        }
    }
}
