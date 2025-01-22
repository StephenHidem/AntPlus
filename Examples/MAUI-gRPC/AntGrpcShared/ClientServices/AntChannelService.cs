using AntChannelGrpcService;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Threading;
using System.Threading.Tasks;
using BasicChannelStatusCode = SmallEarthTech.AntRadioInterface.BasicChannelStatusCode;
using ChannelType = SmallEarthTech.AntRadioInterface.ChannelType;
using ChannelTypeExtended = SmallEarthTech.AntRadioInterface.ChannelTypeExtended;
using MessagingReturnCode = SmallEarthTech.AntRadioInterface.MessagingReturnCode;
using TransmitPower = SmallEarthTech.AntRadioInterface.TransmitPower;

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
        /// Handles channel response updates.
        /// </summary>
        /// <param name="cancellationToken">Cancels subscription to ChannelResponseUpdate.</param>
        public async void HandleChannelResponseUpdates(CancellationToken cancellationToken)
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
            return _client.AssignChannel(new AssignChannelRequest
            {
                ChannelNumber = _channelNumber,
                ChannelType = (AntChannelGrpcService.ChannelType)(uint)channelTypeByte,
                NetworkNumber = networkNumber,
                WaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool AssignChannelExt(ChannelType channelTypeByte, byte networkNumber, ChannelTypeExtended extAssignByte, uint responseWaitTime)
        {
            return _client.AssignChannelExt(new AssignChannelExtRequest
            {
                ChannelNumber = _channelNumber,
                ChannelType = (AntChannelGrpcService.ChannelType)(uint)channelTypeByte,
                NetworkNumber = networkNumber,
                ChannelTypeExtended = (AntChannelGrpcService.ChannelTypeExtended)(uint)extAssignByte,
                WaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool CloseChannel(uint responseWaitTime)
        {
            return _client.CloseChannel(new CloseChannelRequest { ChannelNumber = _channelNumber, WaitTime = responseWaitTime }).Value;
        }

        /// <inheritdoc/>
        public bool ConfigFrequencyAgility(byte freq1, byte freq2, byte freq3, uint responseWaitTime)
        {
            return _client.ConfigureFrequencyAgility(new ConfigureFrequencyAgilityRequest
            {
                ChannelNumber = _channelNumber,
                Frequencies = ByteString.CopyFrom(new byte[] { freq1, freq2, freq3 }),
                WaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public bool IncludeExcludeListAddChannel(ChannelId channelId, byte listIndex, uint responseWaitTime)
        {
            return _client.IncludeExcludeListAddChannel(new IncludeExcludeChannelRequest
            {
                ChannelNumber = _channelNumber,
                ChannelId = channelId.Id,
                ListIndex = listIndex,
                WaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool IncludeExcludeListConfigure(byte listSize, bool isExclusionList, uint responseWaitTime)
        {
            return _client.IncludeExcludeListConfigure(new ConfigureIncludeExcludeChannelListRequest
            {
                ChannelNumber = _channelNumber,
                ListSize = listSize,
                IsExclusionList = isExclusionList,
                WaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool OpenChannel(uint responseWaitTime)
        {
            return _client.OpenChannel(new OpenChannelRequest { ChannelNumber = _channelNumber, WaitTime = responseWaitTime }).Value;
        }

        /// <inheritdoc/>
        public ChannelId RequestChannelID(uint responseWaitTime)
        {
            return new ChannelId(_client.RequestChannelId(new ChannelIdRequest { ChannelNumber = _channelNumber, WaitTime = responseWaitTime }).Value);
        }

        /// <inheritdoc/>
        public ChannelStatus RequestStatus(uint responseWaitTime)
        {
            var reply = _client.RequestChannelStatus(new ChannelStatusRequest { ChannelNumber = _channelNumber, WaitTime = responseWaitTime });
            return new ChannelStatus
            {
                BasicStatus = (BasicChannelStatusCode)reply.BasicChannelStatusCode,
                ChannelType = (ChannelType)reply.ChannelType,
                networkNumber = (byte)reply.NetworkNumber
            };
        }

        /// <inheritdoc/>
        public MessagingReturnCode SendAcknowledgedData(byte[] data, uint ackWaitTime)
        {
            var reply = _client.SendAcknowledgedData(new DataRequest
            {
                ChannelNumber = _channelNumber,
                Data = ByteString.CopyFrom(data),
                WaitTime = ackWaitTime
            });
            return (MessagingReturnCode)reply.ReturnCode;
        }

        /// <inheritdoc/>
        public async Task<MessagingReturnCode> SendAcknowledgedDataAsync(byte[] data, uint ackWaitTime)
        {
            var reply = await _client.SendAcknowledgedDataAsync(new DataRequest
            {
                ChannelNumber = _channelNumber,
                Data = ByteString.CopyFrom(data),
                WaitTime = ackWaitTime
            });
            return (MessagingReturnCode)reply.ReturnCode;
        }

        /// <inheritdoc/>
        public bool SendBroadcastData(byte[] data)
        {
            return _client.SendBroadcastData(new DataRequest
            {
                ChannelNumber = _channelNumber,
                Data = ByteString.CopyFrom(data)
            }).Value;
        }

        /// <inheritdoc/>
        public MessagingReturnCode SendBurstTransfer(byte[] data, uint completeWaitTime)
        {
            var reply = _client.SendBurstTransfer(new DataRequest
            {
                ChannelNumber = _channelNumber,
                Data = ByteString.CopyFrom(data),
                WaitTime = completeWaitTime
            });
            return (MessagingReturnCode)reply.ReturnCode;
        }

        /// <inheritdoc/>
        public async Task<MessagingReturnCode> SendBurstTransferAsync(byte[] data, uint completeWaitTime)
        {
            var reply = await _client.SendBurstTransferAsync(new DataRequest
            {
                ChannelNumber = _channelNumber,
                Data = ByteString.CopyFrom(data),
                WaitTime = completeWaitTime
            });
            return (MessagingReturnCode)reply.ReturnCode;
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
            return _client.SendExtBroadcastData(new ExtDataRequest
            {
                ChannelNumber = _channelNumber,
                ChannelId = channelId.Id,
                Data = ByteString.CopyFrom(data)
            }).Value;
        }

        /// <inheritdoc/>
        public MessagingReturnCode SendExtBurstTransfer(ChannelId channelId, byte[] data, uint completeWaitTime)
        {
            var reply = _client.SendExtBurstTransfer(new ExtDataRequest
            {
                ChannelNumber = _channelNumber,
                ChannelId = channelId.Id,
                Data = ByteString.CopyFrom(data),
                WaitTime = completeWaitTime
            });
            return (MessagingReturnCode)reply.ReturnCode;
        }

        /// <inheritdoc/>
        public async Task<MessagingReturnCode> SendExtBurstTransferAsync(ChannelId channelId, byte[] data, uint completeWaitTime)
        {
            var reply = await _client.SendExtBurstTransferAsync(new ExtDataRequest
            {
                ChannelNumber = _channelNumber,
                ChannelId = channelId.Id,
                Data = ByteString.CopyFrom(data),
                WaitTime = completeWaitTime
            });
            return (MessagingReturnCode)reply.ReturnCode;
        }

        /// <inheritdoc/>
        public bool SetChannelFreq(byte RFFreqOffset, uint responseWaitTime)
        {
            return _client.SetChannelFrequency(new SetChannelFrequencyRequest
            {
                ChannelNumber = _channelNumber,
                Frequency = RFFreqOffset,
                WaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool SetChannelID(ChannelId channelId, uint responseWaitTime)
        {
            return _client.SetChannelId(new SetChannelIdRequest
            {
                ChannelNumber = _channelNumber,
                ChannelId = channelId.Id,
                WaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool SetChannelID_UsingSerial(ChannelId channelId, uint waitResponseTime)
        {
            throw new NotImplementedException();    // TODO: Implement this method
        }

        /// <inheritdoc/>
        public bool SetChannelPeriod(ushort messagePeriod, uint responseWaitTime)
        {
            return _client.SetChannelPeriod(new SetChannelPeriodRequest
            {
                ChannelNumber = _channelNumber,
                Period = messagePeriod,
                WaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool SetChannelSearchTimeout(byte searchTimeout, uint responseWaitTime)
        {
            return _client.SetChannelSearchTimeout(new SetChannelSearchTimeoutRequest
            {
                ChannelNumber = _channelNumber,
                SearchTimeout = searchTimeout,
                WaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool SetChannelTransmitPower(TransmitPower transmitPower, uint responseWaitTime)
        {
            return _client.SetChannelTransmitPower(new SetTransmitPowerRequest
            {
                ChannelNumber = _channelNumber,
                TransmitPower = (AntChannelGrpcService.TransmitPower)transmitPower,
                WaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool SetLowPrioritySearchTimeout(byte lowPriorityTimeout, uint responseWaitTime)
        {
            return _client.SetLowPriorityChannelSearchTimeout(new SetLowPrioritySearchTimeoutRequest
            {
                ChannelNumber = _channelNumber,
                SearchTimeout = lowPriorityTimeout,
                WaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool SetProximitySearch(byte thresholdBin, uint responseWaitTime)
        {
            return _client.SetProximitySearch(new SetProximitySearchRequest
            {
                ChannelNumber = _channelNumber,
                SearchThreshold = thresholdBin,
                WaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool SetSearchThresholdRSSI(byte thresholdRSSI, uint responseWaitTime)
        {
            return _client.SetSearchThresholdRssi(new SetSearchThresholdRssiRequest
            {
                ChannelNumber = _channelNumber,
                SearchThresholdRssi = thresholdRSSI,
                WaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool UnassignChannel(uint responseWaitTime)
        {
            return _client.UnassignChannel(new UnassignChannelRequest { ChannelNumber = _channelNumber, WaitTime = responseWaitTime }).Value;
        }
    }
}
