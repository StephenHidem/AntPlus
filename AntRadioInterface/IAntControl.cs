namespace SmallEarthTech.AntRadioInterface
{
    /// <summary>After desirable configuration of an ANT channel or channels, the control messages provide a method for supervising the RF as well as the activity of the ANT system.</summary>
    public interface IAntControl
    {
        /// <summary>Opens the Rx scan mode.</summary>
        void OpenRxScanMode();
        /// <summary>Opens the Rx scan mode.</summary>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool OpenRxScanMode(uint responseWaitTime);
        /// <summary>Requests the message.</summary>
        /// <param name="channelNum">The channel number.</param>
        /// <param name="messageID">The message identifier.</param>
        void RequestMessage(byte channelNum, RequestMessageID messageID);
        /// <summary>Requests the message.</summary>
        /// <param name="messageID">The message identifier.</param>
        void RequestMessage(RequestMessageID messageID);
        /// <summary>Resets the system.</summary>
        void ResetSystem();
        /// <summary>Resets the system.</summary>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool ResetSystem(uint responseWaitTime);
    }
}
