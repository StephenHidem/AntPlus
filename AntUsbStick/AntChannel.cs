using ANT_Managed_Library;
using AntRadioInterface;
using System;
using System.Linq;

namespace AntUsbStick
{
    public class AntChannel : IAntChannel
    {
        private readonly ANT_Channel antChannel;

        public event EventHandler<IAntResponse> ChannelResponse;

        internal AntChannel(ANT_Channel channel)
        {
            antChannel = channel;
            channel.channelResponse += Channel_channelResponse;
        }

        private void Channel_channelResponse(ANT_Response response)
        {
            ChannelResponse?.Invoke(this, new AntResponse(response));
        }

        public bool AssignChannel(ChannelType channelTypeByte, byte networkNumber, uint responseWaitTime)
        {
            return antChannel.assignChannel((ANT_ReferenceLibrary.ChannelType)channelTypeByte, networkNumber, responseWaitTime);
        }

        public bool AssignChannelExt(ChannelType channelTypeByte, byte networkNumber, ChannelTypeExtended extAssignByte, uint responseWaitTime)
        {
            return antChannel.assignChannelExt((ANT_ReferenceLibrary.ChannelType)channelTypeByte, networkNumber, (ANT_ReferenceLibrary.ChannelTypeExtended)extAssignByte, responseWaitTime);
        }

        public bool CloseChannel(uint responseWaitTime)
        {
            return antChannel.closeChannel(responseWaitTime);
        }

        public bool ConfigFrequencyAgility(byte freq1, byte freq2, byte freq3, uint responseWaitTime)
        {
            return antChannel.configFrequencyAgility(freq1, freq2, freq3, responseWaitTime);
        }

        public bool IncludeExcludeListAddChannel(ushort deviceNumber, byte deviceTypeID, byte transmissionTypeID, byte listIndex, uint responseWaitTime)
        {
            return antChannel.includeExcludeList_addChannel(deviceNumber, deviceTypeID, transmissionTypeID, listIndex, responseWaitTime);
        }

        public bool IncludeExcludeListConfigure(byte listSize, bool isExclusionList, uint responseWaitTime)
        {
            return antChannel.includeExcludeList_Configure(listSize, isExclusionList, responseWaitTime);
        }

        public bool OpenChannel(uint responseWaitTime)
        {
            return antChannel.openChannel(responseWaitTime);
        }

        public ChannelId RequestID(uint responseWaitTime)
        {
            var channelId = antChannel.requestID(responseWaitTime);
            byte[] bytes = { channelId.deviceTypeID, channelId.transmissionTypeID };
            byte[] id = BitConverter.GetBytes(channelId.deviceNumber).Concat(bytes).ToArray();
            return new ChannelId(BitConverter.ToUInt32(id, 0));
        }

        public ChannelStatus RequestStatus(uint responseWaitTime)
        {
            var status = antChannel.requestStatus(responseWaitTime);
            return new ChannelStatus((BasicChannelStatusCode)status.BasicStatus, status.networkNumber, (ChannelType)status.ChannelType);
        }

        public MessagingReturnCode SendAcknowledgedData(byte[] data, uint ackWaitTime)
        {
            return (MessagingReturnCode)antChannel.sendAcknowledgedData((byte[])data, ackWaitTime);
        }

        public bool SendBroadcastData(byte[] data)
        {
            return antChannel.sendBroadcastData(data);
        }

        public MessagingReturnCode SendBurstTransfer(byte[] data, uint completeWaitTime)
        {
            return (MessagingReturnCode)antChannel.sendBurstTransfer(data, completeWaitTime);
        }

        public MessagingReturnCode SendExtAcknowledgedData(ushort deviceNumber, byte deviceTypeID, byte transmissionTypeID, byte[] data, uint ackWaitTime)
        {
            return (MessagingReturnCode)antChannel.sendExtAcknowledgedData(deviceNumber, deviceTypeID, transmissionTypeID, data, ackWaitTime);
        }

        public bool SendExtBroadcastData(ushort deviceNumber, byte deviceTypeID, byte transmissionTypeID, byte[] data)
        {
            return antChannel.sendExtBroadcastData(deviceNumber, deviceTypeID, transmissionTypeID, data);
        }

        public MessagingReturnCode SendExtBurstTransfer(ushort deviceNumber, byte deviceTypeID, byte transmissionTypeID, byte[] data, uint completeWaitTime)
        {
            return (MessagingReturnCode)antChannel.sendExtBurstTransfer(deviceNumber, deviceTypeID, transmissionTypeID, data, completeWaitTime);
        }

        public bool SetChannelFreq(byte RFFreqOffset, uint responseWaitTime)
        {
            return antChannel.setChannelFreq(RFFreqOffset, responseWaitTime);
        }

        /// <summary>Set the channel ID of this channel.
        /// Throws exception if device type is &gt; 127.</summary>
        /// <param name="channelId"></param>
        /// <param name="responseWaitTime">Time to wait for device success response</param>
        /// <returns>True on success. Note: Always returns true with a response time of 0</returns>
        public bool SetChannelID(ChannelId channelId, uint responseWaitTime)
        {
            return antChannel.setChannelID((ushort)channelId.DeviceNumber, channelId.IsPairingBitSet, channelId.DeviceType, BitConverter.GetBytes(channelId.Id)[3], responseWaitTime);
        }

        public bool SetChannelID_UsingSerial(bool pairingEnabled, byte deviceTypeID, byte transmissionTypeID, uint waitResponseTime)
        {
            return antChannel.setChannelID_UsingSerial(pairingEnabled, deviceTypeID, transmissionTypeID, waitResponseTime);
        }

        public bool SetChannelPeriod(ushort messagePeriod_32768unitspersecond, uint responseWaitTime)
        {
            return antChannel.setChannelPeriod(messagePeriod_32768unitspersecond, responseWaitTime);
        }

        public bool SetChannelSearchTimeout(byte searchTimeout, uint responseWaitTime)
        {
            return antChannel.setChannelSearchTimeout(searchTimeout, responseWaitTime);
        }

        public bool SetChannelTransmitPower(TransmitPower transmitPower, uint responseWaitTime)
        {
            return antChannel.setChannelTransmitPower((ANT_ReferenceLibrary.TransmitPower)transmitPower, responseWaitTime);
        }

        public bool SetLowPrioritySearchTimeout(byte lowPriorityTimeout, uint responseWaitTime)
        {
            return antChannel.setLowPrioritySearchTimeout(lowPriorityTimeout, responseWaitTime);
        }

        public bool SetProximitySearch(byte thresholdBin, uint responseWaitTime)
        {
            return antChannel.setProximitySearch(thresholdBin, responseWaitTime);
        }

        public bool SetSearchThresholdRSSI(byte thresholdRSSI, uint responseWaitTime)
        {
            return antChannel.setSearchThresholdRSSI(thresholdRSSI, responseWaitTime);
        }

        public bool UnassignChannel(uint responseWaitTime)
        {
            return antChannel.unassignChannel(responseWaitTime);
        }
    }
}
