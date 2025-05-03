using SmallEarthTech.AntRadioInterface;

namespace AntGrpcService.Services
{
    /// <summary>
    /// ANT radio subscriber factory.
    /// </summary>
    public interface IAntRadioSubscriberFactory
    {
        /// <summary>
        /// Creates a new instance of the AntRadioSubscriber.
        /// </summary>
        /// <param name="antRadio">The AntRadio instance to be used by the subscriber.</param>
        /// <returns>A new instance of the AntRadioSubscriber.</returns>
        IAntRadioSubscriber CreateAntRadioSubscriber(IAntRadio antRadio);
    }
}
