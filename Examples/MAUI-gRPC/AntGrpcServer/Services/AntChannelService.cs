using AntChannelGrpcService;
using Google.Protobuf;
using Grpc.Core;
using SmallEarthTech.AntRadioInterface;

namespace AntGrpcServer.Services
{
    public class AntChannelService(ILogger<AntChannelService> logger, IAntRadio antRadio) : gRPCAntChannel.gRPCAntChannelBase
    {
        private readonly ILogger<AntChannelService> _logger = logger;
        private readonly IAntRadio _antRadio = antRadio;
        private TaskCompletionSource<AntResponse>? _response;

        public override async Task Subscribe(SubscribeRequest request, IServerStreamWriter<ChannelResponse> responseStream, ServerCallContext context)
        {
            _logger.LogInformation("Subscribe called. {ChannelNumber}", request.ChannelNumber);
            IAntChannel antChannel = _antRadio.GetChannel((int)request.ChannelNumber);
            antChannel.ChannelResponse += AntChannelService_ChannelResponse;
            while (!context.CancellationToken.IsCancellationRequested)
            {
                _response = new TaskCompletionSource<AntResponse>();
                AntResponse channelResponse = await _response.Task;
                try
                {
                    await responseStream.WriteAsync(new ChannelResponse
                    {
                        ChannelId = channelResponse.ChannelId.Id,       // TODO: NULL COALESCE CHANNELID
                        ChannelNumber = channelResponse.ChannelNumber,
                        ThresholdConfigurationValue = channelResponse.ThresholdConfigurationValue,
                        Payload = ByteString.CopyFrom(channelResponse.Payload),
                        ResponseId = channelResponse.ResponseId,
                        Rssi = channelResponse.Rssi,
                        Timestamp = channelResponse.Timestamp
                    });
                }
                catch (InvalidOperationException e)
                {
                    _logger.LogInformation("Subscription closed. {Msg}", e.Message);
                    break;
                }
            }
            antChannel.ChannelResponse -= AntChannelService_ChannelResponse;
            _logger.LogInformation("Subscribe exited. {ChannelNumber}", request.ChannelNumber);
        }

        private void AntChannelService_ChannelResponse(object? sender, AntResponse e)
        {
            if (sender != null && _response != null && !_response.TrySetResult(e))
            {
                ((IAntChannel)sender).ChannelResponse -= AntChannelService_ChannelResponse;
            }
        }

        public override async Task<MessagingCodeReply> SendExtAcknowledgedData(ExtDataRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"{nameof(SendExtAcknowledgedData)}");
            SmallEarthTech.AntRadioInterface.MessagingReturnCode reply = await AntRadioService.AntChannels[request.ChannelNumber].SendExtAcknowledgedData(new ChannelId(request.ChannelId), [.. request.Data], request.WaitTime);
            return new MessagingCodeReply { ReturnCode = (AntChannelGrpcService.MessagingReturnCode)reply };
        }
    }
}
