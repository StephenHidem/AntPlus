using AntChannelGrpcService;
using SmallEarthTech.AntRadioInterface;

namespace MauiAntClientApp.Services
{
    internal class GrpcAntResponse : AntResponse
    {
        public GrpcAntResponse(ChannelResponse response)
        {
            ChannelId = new ChannelId(response.ChannelId);
            ChannelNumber = (byte)response.ChannelNumber;
            ThresholdConfigurationValue = (sbyte)response.ThresholdConfigurationValue;
            Payload = [.. response.Payload];
            ResponseId = (byte)response.ResponseId;
            Rssi = (sbyte)response.Rssi;
            Timestamp = (ushort)response.Timestamp;
        }
    }
}