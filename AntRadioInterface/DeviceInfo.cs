namespace SmallEarthTech.AntRadioInterface
{
    /// <summary>The device information class.</summary>
    public class DeviceInfo
    {
        /// <summary>
        /// USB Device Product Description
        /// </summary>
        public byte[] ProductDescription { get; }

        /// <summary>
        /// USB Device Serial String
        /// </summary>
        public byte[] SerialString { get; }

        /// <summary>Initializes a new instance of the <see cref="DeviceInfo" /> class.</summary>
        /// <param name="productDescription">The product description.</param>
        /// <param name="serialString">The serial string.</param>
        public DeviceInfo(byte[] productDescription, byte[] serialString)
        {
            ProductDescription = productDescription;
            SerialString = serialString;
        }
    }
}
