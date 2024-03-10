using AntChannelGrpcService;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using MessagingReturnCode = SmallEarthTech.AntRadioInterface.MessagingReturnCode;

namespace MauiAntClientApp.Services
{
    internal class AntChannelService : IAntChannel
    {
        private readonly ILogger<AntChannelService> _logger;
        private readonly gRPCAntChannel.gRPCAntChannelClient _client;
        private readonly object _lock = new();
        private event EventHandler<AntResponse>? ResponseReceived;
        private AsyncServerStreamingCall<ChannelResponse>? _response;

        public event EventHandler<AntResponse> ChannelResponse
        {
            add
            {
                lock (_lock)
                {
                    if (ResponseReceived == null)
                    {
                        _logger.LogInformation("Invoke HandleChannelResponseEvents");
                        HandleChannelResponseEvents();
                    }
                    ResponseReceived += value;
                }
            }
            remove
            {
                lock (_lock)
                {
                    ResponseReceived -= value;
                    if (ResponseReceived == null)
                    {
                        _logger.LogInformation("Disposing _response");
                        _response?.Dispose();
                    }
                }
            }
        }


        public byte ChannelNumber { get; }

        public AntChannelService(ILogger<AntChannelService> logger, byte antChannel, GrpcChannel grpcChannel)
        {
            _logger = logger;
            ChannelNumber = antChannel;
            _client = new gRPCAntChannel.gRPCAntChannelClient(grpcChannel);
            _logger.LogInformation("Created AntChannelService, ANT channel {AntChannel}.", antChannel);
        }

        private async void HandleChannelResponseEvents()
        {
            _response = _client.Subscribe(new SubscribeRequest { ChannelNumber = ChannelNumber });
            await foreach (var update in _response.ResponseStream.ReadAllAsync())
            {
                ResponseReceived?.Invoke(this, new GrpcAntResponse(update));
            }
        }

        public bool AssignChannel(ChannelType channelTypeByte, byte networkNumber, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool AssignChannelExt(ChannelType channelTypeByte, byte networkNumber, ChannelTypeExtended extAssignByte, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool CloseChannel(uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool ConfigFrequencyAgility(byte freq1, byte freq2, byte freq3, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing _response");
            _response?.Dispose();
        }

        public bool IncludeExcludeListAddChannel(ChannelId channelId, byte listIndex, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool IncludeExcludeListConfigure(byte listSize, bool isExclusionList, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool OpenChannel(uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public ChannelId RequestChannelID(uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public ChannelStatus RequestStatus(uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public MessagingReturnCode SendAcknowledgedData(byte[] data, uint ackWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool SendBroadcastData(byte[] data)
        {
            throw new NotImplementedException();
        }

        public MessagingReturnCode SendBurstTransfer(byte[] data, uint completeWaitTime)
        {
            throw new NotImplementedException();
        }

        public async Task<MessagingReturnCode> SendExtAcknowledgedData(ChannelId channelId, byte[] data, uint ackWaitTime)
        {
            var reply = await _client.SendExtAcknowledgedDataAsync(new ExtDataRequest
            {
                ChannelNumber = ChannelNumber,
                ChannelId = channelId.Id,
                Data = ByteString.CopyFrom(data),
                WaitTime = ackWaitTime
            });
            return (MessagingReturnCode)reply.ReturnCode;
        }

        public bool SendExtBroadcastData(ChannelId channelId, byte[] data)
        {
            throw new NotImplementedException();
        }

        public MessagingReturnCode SendExtBurstTransfer(ChannelId channelId, byte[] data, uint completeWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool SetChannelFreq(byte RFFreqOffset, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool SetChannelID(ChannelId channelId, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool SetChannelID_UsingSerial(ChannelId channelId, uint waitResponseTime)
        {
            throw new NotImplementedException();
        }

        public bool SetChannelPeriod(ushort messagePeriod, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool SetChannelSearchTimeout(byte searchTimeout, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool SetChannelTransmitPower(TransmitPower transmitPower, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool SetLowPrioritySearchTimeout(byte lowPriorityTimeout, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool SetProximitySearch(byte thresholdBin, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool SetSearchThresholdRSSI(byte thresholdRSSI, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool UnassignChannel(uint responseWaitTime)
        {
            throw new NotImplementedException();
        }
    }
}
