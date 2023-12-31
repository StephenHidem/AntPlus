using SmallEarthTech.AntRadioInterface;

namespace SmallEarthTech.AntUsbStick
{
    public partial class AntRadio : IFitSettings
    {
        /// <inheritdoc/>
        public void FitAdjustPairingSettings(byte searchLv, byte pairLv, byte trackLv) => _antDevice.fitAdjustPairingSettings(searchLv, pairLv, trackLv);

        /// <inheritdoc/>
        public bool FitAdjustPairingSettings(byte searchLv, byte pairLv, byte trackLv, uint responseWaitTime) => _antDevice.fitAdjustPairingSettings(searchLv, pairLv, trackLv, responseWaitTime);

        /// <inheritdoc/>
        public void FitSetFEState(byte feState) => _antDevice.fitSetFEState(feState);

        /// <inheritdoc/>
        public bool FitSetFEState(byte feState, uint responseWaitTime) => _antDevice.fitSetFEState(feState, responseWaitTime);
    }
}
