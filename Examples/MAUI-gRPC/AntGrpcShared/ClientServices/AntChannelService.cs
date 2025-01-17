using AntChannelGrpcService;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Threading;
using System.Threading.Tasks;
using MessagingReturnCode = SmallEarthTech.AntRadioInterface.MessagingReturnCode;

namespace AntGrpcShared.ClientServices
{
    /// <summary>  
    /// Service for managing ANT channels.  
    /// </summary>  
    public class AntChannelService : IAntChannel
    {
        private readonly ILogger _logger;
        private readonly byte _channelNumber;
        private readonly GrpcChannel _grpcChannel;
        private readonly gRPCAntChannel.gRPCAntChannelClient _client;

        /// <inheritdoc/>
        public byte ChannelNumber => _channelNumber;

        /// <inheritdoc/>
        public event EventHandler<AntResponse>? ChannelResponse;

        /// <summary>  
        /// Initializes a new instance of the <see cref="AntChannelService"/> class.  
        /// </summary>  
        /// <param name="logger">The logger instance.</param>  
        /// <param name="channelNumber">The channel number.</param>  
        /// <param name="grpcChannel">The gRPC channel.</param>  
        public AntChannelService(ILogger logger, byte channelNumber, GrpcChannel grpcChannel)
        {
            _logger = logger;
            _channelNumber = channelNumber;
            _grpcChannel = grpcChannel;
            _client = new gRPCAntChannel.gRPCAntChannelClient(_grpcChannel);
        }

        /// <summary>
        /// Handles channel response events.
        /// </summary>
        /// <param name="cancellationToken">Cancels subscription to ChannelResponseUpdate.</param>
        public async void HandleChannelResponseEvents(CancellationToken cancellationToken)
        {
            using var response = _client.Subscribe(new SubscribeRequest { ChannelNumber = _channelNumber }, cancellationToken: cancellationToken);
            try
            {
                await foreach (ChannelResponseUpdate? update in response.ResponseStream.ReadAllAsync(cancellationToken))
                {
                    ChannelResponse?.Invoke(this, new GrpcAntResponse(update));
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                _logger.LogInformation("RpcException: operation cancelled");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("OperationCanceledException");
            }
        }

        /// <inheritdoc/>
        public bool AssignChannel(ChannelType channelTypeByte, byte networkNumber, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool AssignChannelExt(ChannelType channelTypeByte, byte networkNumber, ChannelTypeExtended extAssignByte, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool CloseChannel(uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool ConfigFrequencyAgility(byte freq1, byte freq2, byte freq3, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public bool IncludeExcludeListAddChannel(ChannelId channelId, byte listIndex, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool IncludeExcludeListConfigure(byte listSize, bool isExclusionList, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool OpenChannel(uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ChannelId RequestChannelID(uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ChannelStatus RequestStatus(uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public MessagingReturnCode SendAcknowledgedData(byte[] data, uint ackWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<MessagingReturnCode> SendAcknowledgedDataAsync(byte[] data, uint ackWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool SendBroadcastData(byte[] data)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public MessagingReturnCode SendBurstTransfer(byte[] data, uint completeWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<MessagingReturnCode> SendBurstTransferAsync(byte[] data, uint completeWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public MessagingReturnCode SendExtAcknowledgedData(ChannelId channelId, byte[] data, uint ackWaitTime)
        {
            var reply = _client.SendExtAcknowledgedData(new ExtDataRequest
            {
                ChannelNumber = _channelNumber,
                ChannelId = channelId.Id,
                Data = ByteString.CopyFrom(data),
                WaitTime = ackWaitTime
            });
            return (MessagingReturnCode)reply.ReturnCode;
        }

        /// <inheritdoc/>
        public async Task<MessagingReturnCode> SendExtAcknowledgedDataAsync(ChannelId channelId, byte[] data, uint ackWaitTime)
        {
            var reply = await _client.SendExtAcknowledgedDataAsync(new ExtDataRequest
            {
                ChannelNumber = _channelNumber,
                ChannelId = channelId.Id,
                Data = ByteString.CopyFrom(data),
                WaitTime = ackWaitTime
            });
            return (MessagingReturnCode)reply.ReturnCode;
        }

        /// <inheritdoc/>
        public bool SendExtBroadcastData(ChannelId channelId, byte[] data)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public MessagingReturnCode SendExtBurstTransfer(ChannelId channelId, byte[] data, uint completeWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<MessagingReturnCode> SendExtBurstTransferAsync(ChannelId channelId, byte[] data, uint completeWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool SetChannelFreq(byte RFFreqOffset, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool SetChannelID(ChannelId channelId, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool SetChannelID_UsingSerial(ChannelId channelId, uint waitResponseTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool SetChannelPeriod(ushort messagePeriod, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool SetChannelSearchTimeout(byte searchTimeout, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool SetChannelTransmitPower(TransmitPower transmitPower, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool SetLowPrioritySearchTimeout(byte lowPriorityTimeout, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool SetProximitySearch(byte thresholdBin, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool SetSearchThresholdRSSI(byte thresholdRSSI, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool UnassignChannel(uint responseWaitTime)
        {
            throw new NotImplementedException();
        }
    }
}
