namespace SmallEarthTech.AntRadioInterface
{
    /// <summary>This interface is used for ANT radios specifically connected to the host USB ports.</summary>
    public interface IAntUsbRadio
    {
        /// <summary>Gets the opened USB baud rate.</summary>
        /// <returns>The USB baud rate of this device.</returns>
        uint GetBaudRate();

        /// <summary>Gets the opened USB device number.</summary>
        /// <returns>The device number of this device.</returns>
        int GetDeviceNum();

        /// <summary>Gets the device USB PID.</summary>
        /// <returns>The product ID of this device.</returns>
        ushort GetPID();

        /// <summary>Gets the device USB VID.</summary>
        /// <returns>The vendor ID of this device.</returns>
        ushort GetVID();

        /// <summary>Gets the device USB information.</summary>
        /// <returns>
        /// Device information
        /// </returns>
        IDeviceInfo GetDeviceUSBInfo();
        /// <summary>Gets the device USB information.</summary>
        /// <param name="deviceNum">The device number.</param>
        /// <returns>
        /// Device information
        /// </returns>
        IDeviceInfo GetDeviceUSBInfo(byte deviceNum);
        /// <summary>Resets the ANT radio USB interface.</summary>
        void ResetUSB();
    }
}
