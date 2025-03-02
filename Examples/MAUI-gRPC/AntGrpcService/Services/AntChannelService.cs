using AntChannelGrpcService;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
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
                    ResponseId = (uint)channelResponse.ResponseId,
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
        /// <param name="token">Client cancellation token</param>
        /// <returns>Task that completes when canceled.</returns>
        private static Task AwaitCancellation(CancellationToken token)
        {
            var completion = new TaskCompletionSource();
            token.Register(() => completion.SetResult());
            return completion.Task;
        }

        public override Task<BoolValue> AssignChannel(AssignChannelRequest request, ServerCallContext context)
        {
            bool result = AntRadioService.AntChannels[request.ChannelNumber].AssignChannel((SmallEarthTech.AntRadioInterface.ChannelType)request.ChannelType, (byte)request.NetworkNumber, request.WaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        public override Task<BoolValue> AssignChannelExt(AssignChannelExtRequest request, ServerCallContext context)
        {
            bool result = AntRadioService.AntChannels[request.ChannelNumber].AssignChannelExt((SmallEarthTech.AntRadioInterface.ChannelType)request.ChannelType, (byte)request.NetworkNumber, (SmallEarthTech.AntRadioInterface.ChannelTypeExtended)request.ChannelTypeExtended, request.WaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        public override Task<BoolValue> CloseChannel(CloseChannelRequest request, ServerCallContext context)
        {
            bool result = AntRadioService.AntChannels[request.ChannelNumber].CloseChannel(request.WaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        public override Task<BoolValue> ConfigureFrequencyAgility(ConfigureFrequencyAgilityRequest request, ServerCallContext context)
        {
            bool result = AntRadioService.AntChannels[request.ChannelNumber].ConfigFrequencyAgility(request.Frequencies[0], request.Frequencies[1], request.Frequencies[2], request.WaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        public override Task<BoolValue> IncludeExcludeListAddChannel(IncludeExcludeChannelRequest request, ServerCallContext context)
        {
            bool result = AntRadioService.AntChannels[request.ChannelNumber].IncludeExcludeListAddChannel(new ChannelId(request.ChannelId), (byte)request.ListIndex, request.WaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        public override Task<BoolValue> IncludeExcludeListConfigure(ConfigureIncludeExcludeChannelListRequest request, ServerCallContext context)
        {
            bool result = AntRadioService.AntChannels[request.ChannelNumber].IncludeExcludeListConfigure((byte)request.ListSize, request.IsExclusionList, request.WaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        public override Task<BoolValue> OpenChannel(OpenChannelRequest request, ServerCallContext context)
        {
            bool result = AntRadioService.AntChannels[request.ChannelNumber].OpenChannel(request.WaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        public override Task<UInt32Value> RequestChannelId(ChannelIdRequest request, ServerCallContext context)
        {
            ChannelId result = AntRadioService.AntChannels[request.ChannelNumber].RequestChannelID(request.WaitTime);
            return Task.FromResult(new UInt32Value { Value = result.Id });
        }

        public override Task<ChannelStatusReply> RequestChannelStatus(ChannelStatusRequest request, ServerCallContext context)
        {
            ChannelStatus result = AntRadioService.AntChannels[request.ChannelNumber].RequestStatus(request.WaitTime);
            return Task.FromResult(new ChannelStatusReply
            {
                BasicChannelStatusCode = (AntChannelGrpcService.BasicChannelStatusCode)result.BasicStatus,
                NetworkNumber = result.networkNumber,
                ChannelType = (AntChannelGrpcService.ChannelType)result.ChannelType
            });
        }

        public override Task<MessagingCodeReply> SendAcknowledgedData(DataRequest request, ServerCallContext context)
        {
            SmallEarthTech.AntRadioInterface.MessagingReturnCode reply = AntRadioService.AntChannels[request.ChannelNumber].SendAcknowledgedData([.. request.Data], request.WaitTime);
            return Task.FromResult(new MessagingCodeReply { ReturnCode = (AntChannelGrpcService.MessagingReturnCode)reply });
        }

        public override Task<BoolValue> SendBroadcastData(DataRequest request, ServerCallContext context)
        {
            bool result = AntRadioService.AntChannels[request.ChannelNumber].SendBroadcastData([.. request.Data]);
            return Task.FromResult(new BoolValue { Value = result });
        }

        public override Task<MessagingCodeReply> SendBurstTransfer(DataRequest request, ServerCallContext context)
        {
            SmallEarthTech.AntRadioInterface.MessagingReturnCode reply = AntRadioService.AntChannels[request.ChannelNumber].SendBurstTransfer([.. request.Data], request.WaitTime);
            return Task.FromResult(new MessagingCodeReply { ReturnCode = (AntChannelGrpcService.MessagingReturnCode)reply });
        }

        public override async Task<MessagingCodeReply> SendExtAcknowledgedData(ExtDataRequest request, ServerCallContext context)
        {
            SmallEarthTech.AntRadioInterface.MessagingReturnCode reply = await AntRadioService.AntChannels[request.ChannelNumber].SendExtAcknowledgedDataAsync(new ChannelId(request.ChannelId), [.. request.Data], request.WaitTime);
            return new MessagingCodeReply { ReturnCode = (AntChannelGrpcService.MessagingReturnCode)reply };
        }

        public override Task<BoolValue> SendExtBroadcastData(ExtDataRequest request, ServerCallContext context)
        {
            bool result = AntRadioService.AntChannels[request.ChannelNumber].SendExtBroadcastData(new ChannelId(request.ChannelId), [.. request.Data]);
            return Task.FromResult(new BoolValue { Value = result });
        }

        public override async Task<MessagingCodeReply> SendExtBurstTransfer(ExtDataRequest request, ServerCallContext context)
        {
            SmallEarthTech.AntRadioInterface.MessagingReturnCode reply = await AntRadioService.AntChannels[request.ChannelNumber].SendExtBurstTransferAsync(new ChannelId(request.ChannelId), [.. request.Data], request.WaitTime);
            return new MessagingCodeReply { ReturnCode = (AntChannelGrpcService.MessagingReturnCode)reply };
        }

        public override Task<BoolValue> SetChannelFrequency(SetChannelFrequencyRequest request, ServerCallContext context)
        {
            bool result = AntRadioService.AntChannels[request.ChannelNumber].SetChannelFreq((byte)request.Frequency, request.WaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        public override Task<BoolValue> SetChannelId(SetChannelIdRequest request, ServerCallContext context)
        {
            bool result = AntRadioService.AntChannels[request.ChannelNumber].SetChannelID(new ChannelId(request.ChannelId), request.WaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        public override Task<BoolValue> SetChannelPeriod(SetChannelPeriodRequest request, ServerCallContext context)
        {
            bool result = AntRadioService.AntChannels[request.ChannelNumber].SetChannelPeriod((ushort)request.Period, request.WaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        public override Task<BoolValue> SetChannelSearchTimeout(SetChannelSearchTimeoutRequest request, ServerCallContext context)
        {
            bool result = AntRadioService.AntChannels[request.ChannelNumber].SetChannelSearchTimeout((byte)request.SearchTimeout, request.WaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        public override Task<BoolValue> SetChannelTransmitPower(SetTransmitPowerRequest request, ServerCallContext context)
        {
            bool result = AntRadioService.AntChannels[request.ChannelNumber].SetChannelTransmitPower((SmallEarthTech.AntRadioInterface.TransmitPower)request.TransmitPower, request.WaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        public override Task<BoolValue> SetLowPriorityChannelSearchTimeout(SetLowPrioritySearchTimeoutRequest request, ServerCallContext context)
        {
            bool result = AntRadioService.AntChannels[request.ChannelNumber].SetLowPrioritySearchTimeout((byte)request.SearchTimeout, request.WaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        public override Task<BoolValue> SetProximitySearch(SetProximitySearchRequest request, ServerCallContext context)
        {
            bool result = AntRadioService.AntChannels[request.ChannelNumber].SetProximitySearch((byte)request.SearchThreshold, request.WaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        public override Task<BoolValue> SetSearchThresholdRssi(SetSearchThresholdRssiRequest request, ServerCallContext context)
        {
            bool result = AntRadioService.AntChannels[request.ChannelNumber].SetSearchThresholdRSSI((byte)request.SearchThresholdRssi, request.WaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        public override Task<BoolValue> UnassignChannel(UnassignChannelRequest request, ServerCallContext context)
        {
            bool result = AntRadioService.AntChannels[request.ChannelNumber].UnassignChannel(request.WaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }
    }
}
