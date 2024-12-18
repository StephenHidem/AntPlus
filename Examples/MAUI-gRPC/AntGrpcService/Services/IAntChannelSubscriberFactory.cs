using SmallEarthTech.AntRadioInterface;

namespace AntGrpcService.Services
{
    /// <summary>
    /// ANT channel subscriber factory.
    /// </summary>
    public interface IAntChannelSubscriberFactory
    {
        /// <summary>
        /// Creates an ANT channel subscriber.
        /// </summary>
        /// <param name="antChannel">ANT channel associated with this subscriber.</param>
        /// <returns></returns>
        IAntChannelSubscriber CreateAntChannelSubscriber(IAntChannel antChannel);
    }
}
