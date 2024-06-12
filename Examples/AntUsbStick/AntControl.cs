using ANT_Managed_Library;
using SmallEarthTech.AntRadioInterface;

namespace SmallEarthTech.AntUsbStick
{
    public partial class AntRadio : IAntControl
    {
        /// <inheritdoc/>
        public bool OpenRxScanMode(uint responseWaitTime = 0) => _antDevice.openRxScanMode(responseWaitTime);

        /// <inheritdoc/>
        public void RequestMessage(RequestMessageID messageID, byte channelNum = 0) => _antDevice.requestMessage(channelNum, (ANT_ReferenceLibrary.RequestMessageID)messageID);

        /// <inheritdoc/>
        public bool ResetSystem(uint responseWaitTime = 500) => _antDevice.ResetSystem(responseWaitTime);

    }
}
