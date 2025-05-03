using SmallEarthTech.AntRadioInterface;

namespace AntGrpcService.Services
{
    /// <summary>
    /// Declares the ANT radio subscriber interface.
    /// </summary>
    public interface IAntRadioSubscriber : IDisposable
    {
        /// <summary>
        /// This event is raised when any ANT radio response message is received.
        /// </summary>
        event EventHandler<AntResponse> OnAntRadioResponse;
    }
}