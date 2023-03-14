using SmallEarthTech.AntRadioInterface;

namespace SmallEarthTech.AntUsbStick
{
    public partial class AntRadio : IFitSettings
    {
        /// <inheritdoc/>
        public void FitAdjustPairingSettings(byte searchLv, byte pairLv, byte trackLv) => antDevice.fitAdjustPairingSettings(searchLv, pairLv, trackLv);

        /// <inheritdoc/>
        public bool FitAdjustPairingSettings(byte searchLv, byte pairLv, byte trackLv, uint responseWaitTime) => antDevice.fitAdjustPairingSettings(searchLv, pairLv, trackLv, responseWaitTime);

        /// <inheritdoc/>
        public void FitSetFEState(byte feState) => antDevice.fitSetFEState(feState);

        /// <inheritdoc/>
        public bool FitSetFEState(byte feState, uint responseWaitTime) => antDevice.fitSetFEState(feState, responseWaitTime);
    }
}
