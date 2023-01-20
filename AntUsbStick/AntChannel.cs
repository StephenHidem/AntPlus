using ANT_Managed_Library;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Linq;

namespace SmallEarthTech.AntUsbStick
{
    /// <summary>This class contains the implementation of IAntChannel.</summary>
    public class AntChannel : IAntChannel
    {
        private readonly ANT_Channel antChannel;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public bool AssignChannel(ChannelType channelTypeByte, byte networkNumber, uint responseWaitTime)
        {
            return antChannel.assignChannel((ANT_ReferenceLibrary.ChannelType)channelTypeByte, networkNumber, responseWaitTime);
        }

        /// <inheritdoc/>
        public bool AssignChannelExt(ChannelType channelTypeByte, byte networkNumber, ChannelTypeExtended extAssignByte, uint responseWaitTime)
        {
            return antChannel.assignChannelExt((ANT_ReferenceLibrary.ChannelType)channelTypeByte, networkNumber, (ANT_ReferenceLibrary.ChannelTypeExtended)extAssignByte, responseWaitTime);
        }

        /// <inheritdoc/>
        public bool CloseChannel(uint responseWaitTime)
        {
            return antChannel.closeChannel(responseWaitTime);
        }

        /// <inheritdoc/>
        public bool ConfigFrequencyAgility(byte freq1, byte freq2, byte freq3, uint responseWaitTime)
        {
            return antChannel.configFrequencyAgility(freq1, freq2, freq3, responseWaitTime);
        }

        /// <inheritdoc/>
        public bool IncludeExcludeListAddChannel(ChannelId channelId, byte listIndex, uint responseWaitTime)
        {
            return antChannel.includeExcludeList_addChannel((ushort)channelId.DeviceNumber, channelId.DeviceType, BitConverter.GetBytes(channelId.Id)[3], listIndex, responseWaitTime);
        }

        /// <inheritdoc/>
        public bool IncludeExcludeListConfigure(byte listSize, bool isExclusionList, uint responseWaitTime)
        {
            return antChannel.includeExcludeList_Configure(listSize, isExclusionList, responseWaitTime);
        }

        /// <inheritdoc/>
        public bool OpenChannel(uint responseWaitTime)
        {
            return antChannel.openChannel(responseWaitTime);
        }

        /// <summary>Requests the channel identifier.</summary>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public ChannelId RequestID(uint responseWaitTime)
        {
            var channelId = antChannel.requestID(responseWaitTime);
            byte[] bytes = { channelId.deviceTypeID, channelId.transmissionTypeID };
            byte[] id = BitConverter.GetBytes(channelId.deviceNumber).Concat(bytes).ToArray();
            return new ChannelId(BitConverter.ToUInt32(id, 0));
        }

        /// <inheritdoc/>
        public ChannelStatus RequestStatus(uint responseWaitTime)
        {
            var status = antChannel.requestStatus(responseWaitTime);
            return new ChannelStatus((BasicChannelStatusCode)status.BasicStatus, status.networkNumber, (ChannelType)status.ChannelType);
        }

        /// <inheritdoc/>
        public MessagingReturnCode SendAcknowledgedData(byte[] data, uint ackWaitTime)
        {
            return (MessagingReturnCode)antChannel.sendAcknowledgedData((byte[])data, ackWaitTime);
        }

        /// <inheritdoc/>
        public bool SendBroadcastData(byte[] data)
        {
            return antChannel.sendBroadcastData(data);
        }

        /// <inheritdoc/>
        public MessagingReturnCode SendBurstTransfer(byte[] data, uint completeWaitTime)
        {
            return (MessagingReturnCode)antChannel.sendBurstTransfer(data, completeWaitTime);
        }

        /// <inheritdoc/>
        public MessagingReturnCode SendExtAcknowledgedData(ChannelId channelId, byte[] data, uint ackWaitTime)
        {
            return (MessagingReturnCode)antChannel.sendExtAcknowledgedData((ushort)channelId.DeviceNumber, channelId.DeviceType, BitConverter.GetBytes(channelId.Id)[3], data, ackWaitTime);
        }

        /// <inheritdoc/>
        public bool SendExtBroadcastData(ChannelId channelId, byte[] data)
        {
            return antChannel.sendExtBroadcastData((ushort)channelId.DeviceNumber, channelId.DeviceType, BitConverter.GetBytes(channelId.Id)[3], data);
        }

        /// <inheritdoc/>
        public MessagingReturnCode SendExtBurstTransfer(ChannelId channelId, byte[] data, uint completeWaitTime)
        {
            return (MessagingReturnCode)antChannel.sendExtBurstTransfer((ushort)channelId.DeviceNumber, channelId.DeviceType, BitConverter.GetBytes(channelId.Id)[3], data, completeWaitTime);
        }

        /// <inheritdoc/>
        public bool SetChannelFreq(byte RFFreqOffset, uint responseWaitTime)
        {
            return antChannel.setChannelFreq(RFFreqOffset, responseWaitTime);
        }

        /// <inheritdoc/>
        public bool SetChannelID(ChannelId channelId, uint responseWaitTime)
        {
            return antChannel.setChannelID((ushort)channelId.DeviceNumber, channelId.IsPairingBitSet, channelId.DeviceType, BitConverter.GetBytes(channelId.Id)[3], responseWaitTime);
        }

        /// <inheritdoc/>
        public bool SetChannelID_UsingSerial(ChannelId channelId, uint waitResponseTime)
        {
            return antChannel.setChannelID_UsingSerial(channelId.IsPairingBitSet, channelId.DeviceType, BitConverter.GetBytes(channelId.Id)[3], waitResponseTime);
        }

        /// <inheritdoc/>
        public bool SetChannelPeriod(ushort messagePeriod_32768unitspersecond, uint responseWaitTime)
        {
            return antChannel.setChannelPeriod(messagePeriod_32768unitspersecond, responseWaitTime);
        }

        /// <inheritdoc/>
        public bool SetChannelSearchTimeout(byte searchTimeout, uint responseWaitTime)
        {
            return antChannel.setChannelSearchTimeout(searchTimeout, responseWaitTime);
        }

        /// <inheritdoc/>
        public bool SetChannelTransmitPower(TransmitPower transmitPower, uint responseWaitTime)
        {
            return antChannel.setChannelTransmitPower((ANT_ReferenceLibrary.TransmitPower)transmitPower, responseWaitTime);
        }

        /// <inheritdoc/>
        public bool SetLowPrioritySearchTimeout(byte lowPriorityTimeout, uint responseWaitTime)
        {
            return antChannel.setLowPrioritySearchTimeout(lowPriorityTimeout, responseWaitTime);
        }

        /// <inheritdoc/>
        public bool SetProximitySearch(byte thresholdBin, uint responseWaitTime)
        {
            return antChannel.setProximitySearch(thresholdBin, responseWaitTime);
        }

        /// <inheritdoc/>
        public bool SetSearchThresholdRSSI(byte thresholdRSSI, uint responseWaitTime)
        {
            return antChannel.setSearchThresholdRSSI(thresholdRSSI, responseWaitTime);
        }

        /// <inheritdoc/>
        public bool UnassignChannel(uint responseWaitTime)
        {
            return antChannel.unassignChannel(responseWaitTime);
        }
    }
}
