using ANT_Managed_Library;
using SmallEarthTech.AntRadioInterface;

namespace SmallEarthTech.AntUsbStick
{
    public partial class AntRadio : IAntControl
    {
        /// <inheritdoc/>
        public void OpenRxScanMode() => _antDevice.openRxScanMode();

        /// <inheritdoc/>
        public bool OpenRxScanMode(uint responseWaitTime) => _antDevice.openRxScanMode(responseWaitTime);

        /// <inheritdoc/>
        public void RequestMessage(byte channelNum, RequestMessageID messageID) => _antDevice.requestMessage(channelNum, (ANT_ReferenceLibrary.RequestMessageID)messageID);

        /// <inheritdoc/>
        public void RequestMessage(RequestMessageID messageID) => _antDevice.requestMessage((ANT_ReferenceLibrary.RequestMessageID)messageID);

        /// <inheritdoc/>
        public void ResetSystem() => _antDevice.ResetSystem();

        /// <inheritdoc/>
        public bool ResetSystem(uint responseWaitTime) => _antDevice.ResetSystem(responseWaitTime);

    }
}
