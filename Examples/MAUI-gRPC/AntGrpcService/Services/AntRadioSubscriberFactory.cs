using SmallEarthTech.AntRadioInterface;

namespace AntGrpcService.Services
{
    public class AntRadioSubscriberFactory : IAntRadioSubscriberFactory
    {
        /// <summary>
        /// Creates a new instance of the AntRadioSubscriber.
        /// </summary>
        public IAntRadioSubscriber CreateAntRadioSubscriber(IAntRadio antRadio)
        {
            return new AntRadioSubscriber(antRadio);
        }
    }
}
