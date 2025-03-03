using AntChannelGrpcService;
using AntRadioGrpcService;
using SmallEarthTech.AntRadioInterface;

namespace AntGrpcShared.ClientServices
{
    /// <summary>
    /// Represents a gRPC response for an ANT channel.
    /// </summary>
    internal class GrpcAntResponse : AntResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrpcAntResponse"/> class.
        /// </summary>
        /// <param name="response">The channel response update.</param>
        internal GrpcAntResponse(ChannelResponseUpdate response)
        {
            ChannelId = response.ChannelId != null ? new ChannelId((uint)response.ChannelId) : null;
            ChannelNumber = (byte)response.ChannelNumber;
            ThresholdConfigurationValue = (sbyte)response.ThresholdConfigurationValue;
            Payload = response.Payload.ToByteArray();
            ResponseId = (MessageId)response.ResponseId;
            Rssi = (sbyte)response.Rssi;
            Timestamp = (ushort)response.Timestamp;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GrpcAntResponse"/> class.
        /// </summary>
        /// <param name="reply">The ANT response reply.</param>
        internal GrpcAntResponse(AntResponseReply reply)
        {
            ChannelId = new ChannelId(reply.ChannelId);
            ChannelNumber = (byte)reply.ChannelNumber;
            ThresholdConfigurationValue = (sbyte)reply.ThresholdConfigurationValue;
            Payload = reply.Payload.ToByteArray();
            ResponseId = (MessageId)reply.ResponseId;
            Rssi = (sbyte)reply.Rssi;
            Timestamp = (ushort)reply.Timestamp;
        }
    }
}
