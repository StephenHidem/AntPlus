using SmallEarthTech.AntRadioInterface;

namespace AntGrpcService.Services
{
    public class AntRadioSubscriber : IAntRadioSubscriber
    {
        private readonly IAntRadio _antRadio;

        public event EventHandler<AntResponse>? OnAntRadioResponse;

        /// <summary>
        /// Saves the ANT radio associated with this subscriber and connects to the radio response event.
        /// </summary>
        /// <param name="antRadio">ANT radio supplying ANT channel responses.</param>
        public AntRadioSubscriber(IAntRadio antRadio)
        {
            _antRadio = antRadio;
            _antRadio.RadioResponse += ForwardAntResponse;
        }
        /// <summary>
        /// Forward ANT responses to subscribers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ForwardAntResponse(object? sender, AntResponse e)
        {
            OnAntRadioResponse?.Invoke(this, e);
        }

        /// <summary>
        /// Remove our radio response handler.
        /// </summary>
        public void Dispose()
        {
            _antRadio.RadioResponse -= ForwardAntResponse;
            GC.SuppressFinalize(this);
        }
    }
}
