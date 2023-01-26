using System.Linq;
using System.Text;

namespace SmallEarthTech.AntRadioInterface
{
    /// <summary>The device information class. It converts the hyte arrays to strings that can be used in applictations.</summary>
    public class DeviceInfo
    {
        /// <summary>
        /// USB Device Product Description
        /// </summary>
        public string ProductDescription { get; }

        /// <summary>
        /// USB Device Serial String
        /// </summary>
        public string SerialString { get; }

        /// <summary>Initializes a new instance of the <see cref="DeviceInfo" /> class.</summary>
        /// <param name="productDescription">The product description.</param>
        /// <param name="serialString">The serial string.</param>
        public DeviceInfo(byte[] productDescription, byte[] serialString)
        {
            ProductDescription = Encoding.UTF8.GetString(productDescription.TakeWhile(e => e != 0).ToArray());
            SerialString = Encoding.UTF8.GetString(serialString.TakeWhile(e => e != 0).ToArray());
        }
    }
}
