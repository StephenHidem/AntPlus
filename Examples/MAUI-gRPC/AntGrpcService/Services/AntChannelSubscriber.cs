﻿using SmallEarthTech.AntRadioInterface;

namespace AntGrpcService.Services
{
    public sealed class AntChannelSubscriber : IAntChannelSubscriber
    {
        private readonly IAntChannel _antChannel;

        public event EventHandler<AntResponse>? OnAntResponse;

        public AntChannelSubscriber(IAntChannel antChannel)
        {
            _antChannel = antChannel;
            _antChannel.ChannelResponse += SendChannelResponse;
        }

        private void SendChannelResponse(object? sender, AntResponse e)
        {
            OnAntResponse?.Invoke(this, e);
        }

        public void Dispose()
        {
            _antChannel.ChannelResponse -= SendChannelResponse;
        }
    }
}
