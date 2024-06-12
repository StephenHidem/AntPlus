namespace SmallEarthTech.AntRadioInterface
{
    /// <summary>This interface controls general FIT equipped devices.</summary>
    public interface IFitSettings
    {
        /// <summary>FIT adjust pairing settings.</summary>
        /// <param name="searchLv">The search lv.</param>
        /// <param name="pairLv">The pair lv.</param>
        /// <param name="trackLv">The track lv.</param>
        /// <param name="responseWaitTime">The response wait time in milliseconds. The default is 0ms.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool FitAdjustPairingSettings(byte searchLv, byte pairLv, byte trackLv, uint responseWaitTime = 0);
        /// <summary>FIT set the state of the fitness equipment.</summary>
        /// <param name="feState">State of the fe.</param>
        /// <param name="responseWaitTime">The response wait time in milliseconds. The default is 0ms.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool FitSetFEState(byte feState, uint responseWaitTime = 0);
    }
}
