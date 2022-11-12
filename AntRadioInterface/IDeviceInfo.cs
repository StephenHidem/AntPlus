namespace AntRadioInterface
{
    public interface IDeviceInfo
    {
        /// <summary>
        /// USB Device Product Description
        /// </summary>
        byte[] ProductDescription { get; }

        /// <summary>
        /// USB Device Serial String
        /// </summary>
        byte[] SerialString { get; }
    }
}
