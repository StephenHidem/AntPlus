using CommunityToolkit.Mvvm.ComponentModel;
using SmallEarthTech.AntRadioInterface;

namespace MauiAntClientApp.ViewModels
{
    public partial class RadioCapabilitiesViewModel : ObservableObject
    {
        [ObservableProperty]
        private DeviceCapabilities? capabilities;

        [ObservableProperty]
        private List<string> flags = [];

        public RadioCapabilitiesViewModel(IAntRadio radioService)
        {
            Task.Run(async () =>
            {
                Capabilities = await radioService.GetDeviceCapabilities();
                Flags = Capabilities.GetType().GetProperties().
                    Where(t => t.PropertyType == typeof(bool) && (bool)t.GetValue(capabilities)!).
                    ToList().
                    ConvertAll<string>(e => e.Name);
                Flags.Sort();
            });
        }
    }
}
