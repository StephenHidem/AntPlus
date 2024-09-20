using SmallEarthTech.AntRadioInterface;

namespace AntGrpcService.Services
{
    public class AntChannelSubscriberFactory : IAntChannelSubscriberFactory
    {
        public IAntChannelSubscriber CreateAntChannelSubscriber(IAntChannel antChannel)
        {
            return new AntChannelSubscriber(antChannel);
        }
    }
}
