using SmallEarthTech.AntRadioInterface;

namespace SmallEarthTech.AntUsbStick
{
    public partial class AntRadio : IFitSettings
    {

        /// <inheritdoc/>
        public bool FitAdjustPairingSettings(byte searchLv, byte pairLv, byte trackLv, uint responseWaitTime = 0) => _antDevice.fitAdjustPairingSettings(searchLv, pairLv, trackLv, responseWaitTime);

        /// <inheritdoc/>
        public bool FitSetFEState(byte feState, uint responseWaitTime = 0) => _antDevice.fitSetFEState(feState, responseWaitTime);
    }
}
