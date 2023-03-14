namespace SmallEarthTech.AntRadioInterface
{
    public interface IAntUsbRadio
    {
        /// <summary>Gets the opened USB baud rate.</summary>
        uint GetBaudRate();

        /// <summary>Gets the opened USB device number.</summary>
        int GetDeviceNum();

        /// <summary>Gets the device USB PID.</summary>
        ushort GetPID();

        /// <summary>Gets the device USB VID.</summary>
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
