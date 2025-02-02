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
        /// This implementation of IAntChannel supports sending messages to ANT devices. This implementation is very
        /// limited in its support; only <see cref="SendExtAcknowledgedDataAsync(ChannelId, byte[], uint)"/>
        /// is implemented. It is only intended to be used by the <see cref="AntCollection"/> class to provide the IAntChannel
        /// argument when creating ANT devices. All messages to be sent to devices are coordinated by this class.
        /// </summary>
        /// <remarks>
        /// <see cref="AntCollection"/> passes an array of ANT channels to send messages. The messaging strategy is to find a
        /// channel that is not busy and dispatch the message on that channel.
        /// </remarks>
        /// <seealso cref="IAntChannel" />
        private class SendMessageChannel : IAntChannel
        {
            private readonly IAntChannel[] _channels;
            private readonly ILogger<AntCollection> _logger;
            private readonly bool[] _busyFlags;
            private readonly object _channelLock = new object();

            public byte ChannelNumber => throw new NotImplementedException();

            public event EventHandler<AntResponse>? ChannelResponse { add { } remove { } }

            public SendMessageChannel(IAntChannel[] channels, ILogger<AntCollection> logger)
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

            public MessagingReturnCode SendExtAcknowledgedData(ChannelId channelId, byte[] data, uint ackWaitTime) => throw new NotImplementedException();

            public Task<MessagingReturnCode> SendExtAcknowledgedDataAsync(ChannelId channelId, byte[] data, uint ackWaitTime)
            {
                TaskCompletionSource<MessagingReturnCode> tcs = new TaskCompletionSource<MessagingReturnCode>();
                GetAvailableChannelIndexAsync()
                    .ContinueWith(antecedent =>
                    {
                        int index = antecedent.Result;
                        _logger.LogDebug("SendExtAcknowledgedDataAsync: Invoke. Task ID = {TaskId}, channel index = {ChannelIndex}, channel ID = 0x{ChannelId:X8}", antecedent.Id, index, channelId.Id);
                        //Thread.Sleep(1000);
                        _channels[index].SendExtAcknowledgedDataAsync(channelId, data, ackWaitTime)
                        .ContinueWith(innerAntecedent =>
                        {
                            _logger.LogDebug("SendExtAcknowledgedDataAsync: Completed. Task ID = {TaskId}, channel index = {ChannelIndex}, channel ID = 0x{ChannelId:X8}", antecedent.Id, index, channelId.Id);
                            lock (_channelLock)
                            {
                                _busyFlags[index] = false;
                            }
                            tcs.SetResult(innerAntecedent.Result);
                        });
                    });
                return tcs.Task;
            }

            private Task<int> GetAvailableChannelIndexAsync()
            {
                return Task.Run(() =>
                {
                    _logger.LogDebug("GetAvailableChannelIndexAsync: Task ID = {TaskId}, _busyFlags = {BusyFlags}", Task.CurrentId, _busyFlags);
                    int i = -1;
                    while (i == -1)
                    {
                        lock (_channelLock)
                        {
                            i = Array.FindIndex(_busyFlags, flag => !flag);
                            if (i != -1)
                            {
                                _busyFlags[i] = true;
                                break;
                            }
                        }
                        Thread.Sleep(100);
                    }
                    return i;
                });
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
