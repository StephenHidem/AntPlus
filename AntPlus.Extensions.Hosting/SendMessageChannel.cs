using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmallEarthTech.AntPlus.Extensions.Hosting
{
    public partial class AntCollection
    {
        /// <summary>
        /// This implementation of IAntChannel supports sending messages to ANT devices. The implementation is very
        /// limited in its support; only <see cref="SendAcknowledgedData(byte[], uint)"/> and <see cref="SendAcknowledgedDataAsync(byte[], uint)"/>
        /// are implemented. It is only intended to be used by the <see cref="AntCollection"/> class to provide the IAntChannel
        /// argument when creating ANT devices. All messages to be sent to devices are coordinated by this class.
        /// </summary>
        /// <remarks>
        /// <see cref="AntCollection"/> passes an array of ANT channels to send messages. The messaging strategy is to find a channel that is not
        /// busy and dispatch that message on that channel.
        /// </remarks>
        /// <seealso cref="SmallEarthTech.AntRadioInterface.IAntChannel" />
        private class SendMessageChannel : IAntChannel
        {
            private readonly IAntChannel[] _channels;
            private readonly ILogger<AntCollection> _logger;
            private readonly bool[] _busyFlags;

            public byte ChannelNumber => throw new NotImplementedException();

            public event EventHandler<AntResponse>? ChannelResponse { add { } remove { } }

            internal SendMessageChannel(IAntChannel[] channels, ILogger<AntCollection> logger)
            {
                _channels = channels;
                _logger = logger;
                _busyFlags = new bool[channels.Length];
            }

            public bool AssignChannel(ChannelType channelTypeByte, byte networkNumber, uint responseWaitTime) => throw new NotImplementedException();

            public bool AssignChannelExt(ChannelType channelTypeByte, byte networkNumber, ChannelTypeExtended extAssignByte, uint responseWaitTime) => throw new NotImplementedException();

            public bool CloseChannel(uint responseWaitTime) => throw new NotImplementedException();

            public bool ConfigFrequencyAgility(byte freq1, byte freq2, byte freq3, uint responseWaitTime) => throw new NotImplementedException();

            public void Dispose() => throw new NotImplementedException();

            public bool IncludeExcludeListAddChannel(ChannelId channelId, byte listIndex, uint responseWaitTime) => throw new NotImplementedException();

            public bool IncludeExcludeListConfigure(byte listSize, bool isExclusionList, uint responseWaitTime) => throw new NotImplementedException();

            public bool OpenChannel(uint responseWaitTime) => throw new NotImplementedException();

            public ChannelId RequestChannelID(uint responseWaitTime) => throw new NotImplementedException();

            public ChannelStatus RequestStatus(uint responseWaitTime) => throw new NotImplementedException();

            public MessagingReturnCode SendAcknowledgedData(byte[] data, uint ackWaitTime) => throw new NotImplementedException();

            public Task<MessagingReturnCode> SendAcknowledgedDataAsync(byte[] data, uint ackWaitTime) => throw new NotImplementedException();

            public bool SendBroadcastData(byte[] data) => throw new NotImplementedException();

            public MessagingReturnCode SendBurstTransfer(byte[] data, uint completeWaitTime) => throw new NotImplementedException();

            public Task<MessagingReturnCode> SendBurstTransferAsync(byte[] data, uint completeWaitTime) => throw new NotImplementedException();

            public MessagingReturnCode SendExtAcknowledgedData(ChannelId channelId, byte[] data, uint ackWaitTime)
            {
                int index = Task.Run(() =>
                {
                    int i = -1;
                    // block task while all channels are busy
                    while (i == -1)
                    {
                        i = Array.FindIndex(_busyFlags, f => !f);
                        if (i == -1) Thread.Sleep(100);     // release this thread's time slice if we can't find an available channel
                    }
                    _busyFlags[i] = true;
                    return i;
                }).Result;

                _logger.LogDebug("SendExtAcknowledgedData: channel index = {ChannelIndex}, channel ID = 0x{ChanelId:X8}, busyFlags = {BusyFlags}", index, channelId.Id, _busyFlags);
                var result = _channels[index].SendExtAcknowledgedData(
                            channelId,
                            data,
                            ackWaitTime);
                _busyFlags[index] = false;
                return result;
            }

            public async Task<MessagingReturnCode> SendExtAcknowledgedDataAsync(ChannelId channelId, byte[] data, uint ackWaitTime)
            {
                int index = await Task.Run(() =>
                {
                    int i = -1;
                    // block task while all channels are busy
                    while (i == -1)
                    {
                        i = Array.FindIndex(_busyFlags, f => !f);
                        if (i == -1) Thread.Sleep(100);     // release this thread's time slice if we can't find an available channel
                    }
                    _busyFlags[i] = true;
                    return i;
                });

                _logger.LogDebug("SendExtAcknowledgedDataAsync: channel index = {ChannelIndex}, channel ID = 0x{ChannelId:X8}, busyFlags = {BusyFlags}", index, channelId.Id, _busyFlags);
                var result = await _channels[index].SendExtAcknowledgedDataAsync(
                            channelId,
                            data,
                            ackWaitTime);
                _busyFlags[index] = false;
                return result;
            }

            public bool SendExtBroadcastData(ChannelId channelId, byte[] data) => throw new NotImplementedException();

            public MessagingReturnCode SendExtBurstTransfer(ChannelId channelId, byte[] data, uint completeWaitTime) => throw new NotImplementedException();

            public Task<MessagingReturnCode> SendExtBurstTransferAsync(ChannelId channelId, byte[] data, uint completeWaitTime) => throw new NotImplementedException();

            public bool SetChannelFreq(byte RFFreqOffset, uint responseWaitTime) => throw new NotImplementedException();

            public bool SetChannelID(ChannelId channelId, uint responseWaitTime) => throw new NotImplementedException();

            public bool SetChannelID_UsingSerial(ChannelId channelId, uint waitResponseTime) => throw new NotImplementedException();

            public bool SetChannelPeriod(ushort messagePeriod, uint responseWaitTime) => throw new NotImplementedException();

            public bool SetChannelSearchTimeout(byte searchTimeout, uint responseWaitTime) => throw new NotImplementedException();

            public bool SetChannelTransmitPower(TransmitPower transmitPower, uint responseWaitTime) => throw new NotImplementedException();

            public bool SetLowPrioritySearchTimeout(byte lowPriorityTimeout, uint responseWaitTime) => throw new NotImplementedException();

            public bool SetProximitySearch(byte thresholdBin, uint responseWaitTime) => throw new NotImplementedException();

            public bool SetSearchThresholdRSSI(byte thresholdRSSI, uint responseWaitTime) => throw new NotImplementedException();

            public bool UnassignChannel(uint responseWaitTime) => throw new NotImplementedException();
        }
    }
}
