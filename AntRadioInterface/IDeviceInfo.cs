namespace SmallEarthTech.AntRadioInterface
{
    /// <summary>The USB device information interface.</summary>
    public interface IDeviceInfo
    {
        /// <summary>
        /// USB Device Product Description
        /// </summary>
        string ProductDescription { get; }
        /// <summary>
        /// USB Device Serial String
        /// </summary>
        string SerialString { get; }
    }
}