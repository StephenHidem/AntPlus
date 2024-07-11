using CommunityToolkit.Mvvm.ComponentModel;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower
{
    public partial class StandardPowerSensor
    {
        /// <summary>Gets the left leg torque effectiveness as a percentage. NaN indicates invalid.</summary>
        [ObservableProperty]
        private double leftTorqueEffectiveness;
        /// <summary>Gets the right leg torque effectiveness as a percentage. NaN indicates invalid.</summary>
        [ObservableProperty]
        private double rightTorqueEffectiveness;
        /// <summary>Gets the left pedal smoothness as a percentage. NaN indicates invalid.</summary>
        [ObservableProperty]
        private double leftPedalSmoothness;
        /// <summary>Gets the right pedal smoothness as a percentage. NaN indicates invalid. Check <see cref="CombinedPedalSmoothness"/>.</summary>
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
        private void ParseTEPS(byte[] dataPage)
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
                    RightPedalSmoothness = double.NaN;
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
