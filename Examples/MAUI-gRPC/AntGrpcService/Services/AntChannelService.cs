using AntChannelGrpcService;
using Google.Protobuf;
using Grpc.Core;
using SmallEarthTech.AntRadioInterface;

namespace AntGrpcService.Services
{
    public class AntChannelService : gRPCAntChannel.gRPCAntChannelBase
    {
        private readonly ILogger<AntChannelService> _logger;
        private readonly IAntRadio _antRadio;
        private readonly IAntChannelSubscriberFactory _subscriberFactory;
        private readonly TaskCompletionSource<AntResponse>? _response;

        public AntChannelService(ILogger<AntChannelService> logger, IAntRadio antRadio, IAntChannelSubscriberFactory subscriberFactory)
        {
            _logger = logger;
            _antRadio = antRadio;
            _subscriberFactory = subscriberFactory;
        }

        public override async Task Subscribe(SubscribeRequest request, IServerStreamWriter<ChannelResponse> responseStream, ServerCallContext context)
        {
            _logger.LogInformation("Subscribe called. {ChannelNumber}", request.ChannelNumber);
            using IAntChannelSubscriber subscriber = _subscriberFactory.CreateAntChannelSubscriber(_antRadio.GetChannel((int)request.ChannelNumber));
            subscriber.OnAntResponse += async (sender, args) =>
                await WriteUpdateAsync(responseStream, args);

            await AwaitCancellation(context.CancellationToken);

            _logger.LogInformation("Subscribe exited. {ChannelNumber}", request.ChannelNumber);
        }

        private async Task WriteUpdateAsync(IServerStreamWriter<ChannelResponse> responseStream, AntResponse channelResponse)
        {
            try
            {
                await responseStream.WriteAsync(new ChannelResponse
                {
                    ChannelId = channelResponse.ChannelId!.Id,
                    ChannelNumber = channelResponse.ChannelNumber,
                    ThresholdConfigurationValue = channelResponse.ThresholdConfigurationValue,
                    Payload = ByteString.CopyFrom(channelResponse.Payload),
                    ResponseId = channelResponse.ResponseId,
                    Rssi = channelResponse.Rssi,
                    Timestamp = channelResponse.Timestamp
                });
            }
            catch (Exception e)
            {
                // Handle any errors caused by broken connection, etc.
                _logger.LogError(e, "Failed to write message");
            }
        }

        private static Task AwaitCancellation(CancellationToken token)
        {
            var completion = new TaskCompletionSource();
            token.Register(() => completion.SetResult());
            return completion.Task;
        }

        public override async Task<MessagingCodeReply> SendExtAcknowledgedData(ExtDataRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"{nameof(SendExtAcknowledgedData)}");
            SmallEarthTech.AntRadioInterface.MessagingReturnCode reply = await AntRadioService.AntChannels[request.ChannelNumber].SendExtAcknowledgedDataAsync(new ChannelId(request.ChannelId), [.. request.Data], request.WaitTime);
            return new MessagingCodeReply { ReturnCode = (AntChannelGrpcService.MessagingReturnCode)reply };
        }
    }
}
