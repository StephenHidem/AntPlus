using SmallEarthTech.AntRadioInterface;

namespace AntGrpcService.Services
{
    public interface IAntChannelSubscriber : IDisposable
    {
        event EventHandler<AntResponse> OnAntResponse;
    }
}
