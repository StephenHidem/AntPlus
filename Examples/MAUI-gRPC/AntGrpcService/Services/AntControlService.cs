using AntControlGrpcService;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SmallEarthTech.AntRadioInterface;

namespace AntGrpcService.Services
{
    public class AntControlService : gRPCAntControl.gRPCAntControlBase
    {
        private readonly IAntControl _antRadio;

        public AntControlService(IAntRadio antRadio)
        {
            _antRadio = (IAntControl)antRadio;
        }

        public override Task<BoolValue> OpenRxScanMode(OpenRxScanModeRequest request, ServerCallContext context)
        {
            bool result = _antRadio.OpenRxScanMode(request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        public override Task<Empty> RequestMessage(RequestMessageRequest request, ServerCallContext context)
        {
            _antRadio.RequestMessage((SmallEarthTech.AntRadioInterface.RequestMessageID)request.MessageId, (byte)request.ChannelNumber);
            return Task.FromResult(new Empty());
        }

        public override Task<BoolValue> ResetSystem(ResetSystemRequest request, ServerCallContext context)
        {
            bool result = _antRadio.ResetSystem(request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }
    }
}
