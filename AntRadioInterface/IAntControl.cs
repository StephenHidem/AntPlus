namespace SmallEarthTech.AntRadioInterface
{
    /// <summary>This interface defines ANT radio control messages and status requests.</summary>
    public interface IAntControl
    {
        /// <summary>Open channel 0 in continuous scan mode. Channel 0 should have been
        /// previously assigned and configured as a slave receive channel.</summary>
        void OpenRxScanMode();
        /// <summary>Open channel 0 in continuous scan mode. Channel 0 should have been
        /// previously assigned and configured as a slave receive channel.</summary>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool OpenRxScanMode(uint responseWaitTime);
        /// <summary>This message is sent to the ANT radio to request specific information from the radio.</summary>
        /// <param name="channelNum">The channel number.</param>
        /// <param name="messageID">The message identifier.</param>
        void RequestMessage(byte channelNum, RequestMessageID messageID);
        /// <summary>This message is sent to the ANT radio to request specific information from the radio.</summary>
        /// <param name="messageID">The message identifier.</param>
        void RequestMessage(RequestMessageID messageID);
        /// <summary>Resets the ANT radio.</summary>
        /// <remarks>
        /// Execution of this command terminates all channels.All information previously configured
        /// in the system can no longer be considered valid. The application should wait 500ms before any
        /// further commands are issued from the host.
        /// </remarks>
        void ResetSystem();
        /// <summary>Resets the ANT radio.</summary>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        /// <remarks>
        /// Execution of this command terminates all channels.All information previously configured
        /// in the system can no longer be considered valid. The application should wait 500ms before any
        /// further commands are issued from the host.
        /// </remarks>
        bool ResetSystem(uint responseWaitTime);
    }
}
