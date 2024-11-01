using SmallEarthTech.AntRadioInterface;

namespace AntGrpcService.Services
{
    /// <summary>
    /// Implementation of the ANT channel subscriber factory.
    /// </summary>
    public class AntChannelSubscriberFactory : IAntChannelSubscriberFactory
    {
        /// <inheritdoc/>
        public IAntChannelSubscriber CreateAntChannelSubscriber(IAntChannel antChannel)
        {
            return new AntChannelSubscriber(antChannel);
        }
    }
}
