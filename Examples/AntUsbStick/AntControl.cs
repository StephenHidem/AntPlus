using ANT_Managed_Library;
using SmallEarthTech.AntRadioInterface;

namespace SmallEarthTech.AntUsbStick
{
    public partial class AntRadio : IAntControl
    {
        /// <inheritdoc/>
        public void OpenRxScanMode() => antDevice.openRxScanMode();

        /// <inheritdoc/>
        public bool OpenRxScanMode(uint responseWaitTime) => antDevice.openRxScanMode(responseWaitTime);

        /// <inheritdoc/>
        public void RequestMessage(byte channelNum, RequestMessageID messageID) => antDevice.requestMessage(channelNum, (ANT_ReferenceLibrary.RequestMessageID)messageID);

        /// <inheritdoc/>
        public void RequestMessage(RequestMessageID messageID) => antDevice.requestMessage((ANT_ReferenceLibrary.RequestMessageID)messageID);

        /// <inheritdoc/>
        public void ResetSystem() => antDevice.ResetSystem();

        /// <inheritdoc/>
        public bool ResetSystem(uint responseWaitTime) => antDevice.ResetSystem(responseWaitTime);

    }
}
