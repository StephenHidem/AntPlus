using SmallEarthTech.AntRadioInterface;

namespace AntGrpcService.Services
{
    /// <summary>
    /// Declares the ANT channel subscriber interface.
    /// </summary>
    public interface IAntChannelSubscriber : IDisposable
    {
        /// <summary>
        /// This event is raised when any ANT channel response message is recieved.
        /// </summary>
        event EventHandler<AntResponse> OnAntResponse;
    }
}
