namespace SmallEarthTech.AntRadioInterface
{
    /// <summary>This interface defines ANT radio control messages and status requests.</summary>
    public interface IAntControl
    {
        /// <summary>Open channel 0 in continuous scan mode. Channel 0 should have been
        /// previously assigned and configured as a slave receive channel.</summary>
        /// <param name="responseWaitTime">The response wait time in milliseconds. The default is 0ms.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool OpenRxScanMode(uint responseWaitTime = 0);
        /// <summary>This message is sent to the ANT radio to request specific information from the radio.</summary>
        /// <param name="messageID">The message identifier.</param>
        /// <param name="channelNum">The channel number. The default channel is 0.</param>
        void RequestMessage(RequestMessageID messageID, byte channelNum = 0);
        /// <summary>Resets the ANT radio.</summary>
        /// <param name="responseWaitTime">The response wait time in milliseconds. The default is 500ms.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        /// <remarks>
        /// Execution of this command terminates all channels.All information previously configured
        /// in the system can no longer be considered valid. The application should wait 500ms before any
        /// further commands are issued from the host.
        /// </remarks>
        bool ResetSystem(uint responseWaitTime = 500);
    }
}
