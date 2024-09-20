using SmallEarthTech.AntRadioInterface;

namespace AntGrpcService.Services
{
    public interface IAntChannelSubscriberFactory
    {
        IAntChannelSubscriber CreateAntChannelSubscriber(IAntChannel antChannel);
    }
}
