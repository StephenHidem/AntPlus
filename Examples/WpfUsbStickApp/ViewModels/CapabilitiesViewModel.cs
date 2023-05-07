using SmallEarthTech.AntRadioInterface;
using System.Collections.Generic;
using System.Linq;

namespace WpfUsbStickApp.ViewModels
{
    internal class CapabilitiesViewModel
    {
        private readonly DeviceCapabilities capabilities;

        public byte MaxANTChannels => capabilities.MaxANTChannels;
        public byte MaxNetworks => capabilities.MaxNetworks;
        public byte MaxDataChannels => capabilities.MaxDataChannels;
        public List<string> Capabilities { get; private set; }

        public CapabilitiesViewModel(DeviceCapabilities capabilities)
        {
            this.capabilities = capabilities;
            Capabilities = capabilities.GetType().GetProperties().
                Where(t => t.PropertyType == typeof(bool) && (bool)t.GetValue(capabilities)!).
                ToList().
                ConvertAll<string>(e => e.Name);
        }
    }
}
