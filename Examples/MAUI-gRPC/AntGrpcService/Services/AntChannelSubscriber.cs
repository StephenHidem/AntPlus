using SmallEarthTech.AntRadioInterface;

namespace AntGrpcService.Services
{
    /// <summary>
    /// Implements the ANT channel subscriber interface.
    /// </summary>
    public sealed class AntChannelSubscriber : IAntChannelSubscriber
    {
        private readonly IAntChannel _antChannel;

        /// <inheritdoc/>
        public event EventHandler<AntResponse>? OnAntResponse;

        /// <summary>
        /// Saves the ANT channel associated with this subscriber and connects to the channel response event.
        /// </summary>
        /// <param name="antChannel">ANT channel supplying ANT channel responses.</param>
        public AntChannelSubscriber(IAntChannel antChannel)
        {
            _antChannel = antChannel;
            _antChannel.ChannelResponse += SendChannelResponse;
        }

        /// <summary>
        /// Forward ANT responses to subscribers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendChannelResponse(object? sender, AntResponse e)
        {
            OnAntResponse?.Invoke(this, e);
        }

        /// <summary>
        /// Remove our channel response handler.
        /// </summary>
        public void Dispose()
        {
            _antChannel.ChannelResponse -= SendChannelResponse;
        }
    }
}
