using AntControlGrpcService;
using SmallEarthTech.AntRadioInterface;

namespace AntGrpcShared.ClientServices
{
    public partial class AntRadioService : IAntControl
    {
        private gRPCAntControl.gRPCAntControlClient? _control;

        /// <inheritdoc/>
        public bool OpenRxScanMode(uint responseWaitTime = 0)
        {
            return _control!.OpenRxScanMode(new OpenRxScanModeRequest { ResponseWaitTime = responseWaitTime }).Value;
        }

        /// <inheritdoc/>
        public void RequestMessage(SmallEarthTech.AntRadioInterface.RequestMessageID messageID, byte channelNum = 0)
        {
            _control!.RequestMessage(new RequestMessageRequest { MessageId = (AntControlGrpcService.RequestMessageID)messageID, ChannelNumber = channelNum });
        }

        /// <inheritdoc/>
        public bool ResetSystem(uint responseWaitTime = 500)
        {
            return _control!.ResetSystem(new ResetSystemRequest { ResponseWaitTime = responseWaitTime }).Value;
        }
    }
}
