using AntChannelGrpcService;
using SmallEarthTech.AntRadioInterface;

namespace AntGrpcShared.ClientServices
{
    /// <summary>
    /// Represents a gRPC response for an ANT channel.
    /// </summary>
    public class GrpcAntResponse : AntResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrpcAntResponse"/> class.
        /// </summary>
        /// <param name="response">The channel response update.</param>
        public GrpcAntResponse(ChannelResponseUpdate response)
        {
            ChannelId = response.ChannelId != null ? new ChannelId((uint)response.ChannelId) : null;
            ChannelNumber = (byte)response.ChannelNumber;
            ThresholdConfigurationValue = (sbyte)response.ThresholdConfigurationValue;
            Payload = response.Payload.ToByteArray();
            ResponseId = (byte)response.ResponseId;
            Rssi = (sbyte)response.Rssi;
            Timestamp = (ushort)response.Timestamp;
        }
    }
}
