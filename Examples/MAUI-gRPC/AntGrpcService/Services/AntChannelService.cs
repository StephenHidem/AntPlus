using AntChannelGrpcService;
using Google.Protobuf;
using Grpc.Core;
using SmallEarthTech.AntRadioInterface;

namespace AntGrpcService.Services
{
    /// <summary>
    /// Server-side implementation of gRPCAntChannel.
    /// </summary>
    /// <param name="logger">AntChannelService logger.</param>
    /// <param name="antRadio">ANT radio to use.</param>
    /// <param name="subscriberFactory">ANT channel subscriber factory.</param>
    public class AntChannelService(ILogger<AntChannelService> logger, IAntRadio antRadio, IAntChannelSubscriberFactory subscriberFactory) : gRPCAntChannel.gRPCAntChannelBase
    {
        public override async Task Subscribe(SubscribeRequest request, IServerStreamWriter<ChannelResponseUpdate> responseStream, ServerCallContext context)
        {
            logger.LogInformation("Subscriber entered. Channel number = {ChannelNumber}, Peer = {Peer}", request.ChannelNumber, context.Peer);
            using IAntChannelSubscriber subscriber = subscriberFactory.CreateAntChannelSubscriber(antRadio.GetChannel((int)request.ChannelNumber));

            // create a response handler delegate and add it to subscriber
            async void handler(object? sender, AntResponse args) => await WriteUpdateAsync(responseStream, args);
            subscriber.OnAntResponse += handler;

            await AwaitCancellation(context.CancellationToken);

            // remove our response handler from the subscriber
            subscriber.OnAntResponse -= handler;

            logger.LogInformation("Subscriber exited. Channel number = {ChannelNumber}, Peer = {Peer}", request.ChannelNumber, context.Peer);
        }

        /// <summary>
        /// Writes the ANT response update to the stream.
        /// </summary>
        /// <param name="responseStream">Subscribed stream.</param>
        /// <param name="channelResponse">ANT channel response received.</param>
        /// <returns>Task to await</returns>
        private async Task WriteUpdateAsync(IServerStreamWriter<ChannelResponseUpdate> responseStream, AntResponse channelResponse)
        {
            try
            {
                await responseStream.WriteAsync(new ChannelResponseUpdate
                {
                    // build channel response update message
                    ChannelId = channelResponse.ChannelId?.Id,
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
                logger.LogError(e, "Failed to write message. Channel = {ChannelNumber}", channelResponse.ChannelNumber);
            }
        }

        /// <summary>
        /// This task completes when the connection is closed by the client.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Task that completes when canceled.</returns>
        private static Task AwaitCancellation(CancellationToken token)
        {
            var completion = new TaskCompletionSource();
            token.Register(() => completion.SetResult());
            return completion.Task;
        }

        public override async Task<MessagingCodeReply> SendExtAcknowledgedData(ExtDataRequest request, ServerCallContext context)
        {
            SmallEarthTech.AntRadioInterface.MessagingReturnCode reply = await AntRadioService.AntChannels[request.ChannelNumber].SendExtAcknowledgedDataAsync(new ChannelId(request.ChannelId), [.. request.Data], request.WaitTime);
            logger.LogDebug("SendExtAcknowledgedData: Reply = {Reply}", reply);
            return new MessagingCodeReply { ReturnCode = (AntChannelGrpcService.MessagingReturnCode)reply };
        }
    }
}
