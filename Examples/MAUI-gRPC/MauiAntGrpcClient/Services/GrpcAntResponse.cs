using AntChannelGrpcService;
using SmallEarthTech.AntRadioInterface;

namespace MauiAntGrpcClient.Services
{
    internal class GrpcAntResponse : AntResponse
    {
        public GrpcAntResponse(ChannelResponseUpdate response)
        {
            ChannelId = response.ChannelId != null ? new ChannelId((uint)response.ChannelId) : null;
            ChannelNumber = (byte)response.ChannelNumber;
            ThresholdConfigurationValue = (sbyte)response.ThresholdConfigurationValue;
            Payload = [.. response.Payload];
            ResponseId = (byte)response.ResponseId;
            Rssi = (sbyte)response.Rssi;
            Timestamp = (ushort)response.Timestamp;
        }
    }
}
