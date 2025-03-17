using CommunityToolkit.Mvvm.ComponentModel;
using SmallEarthTech.AntRadioInterface;

namespace MauiAntGrpcClient.ViewModels
{
    public partial class RadioCapabilitiesViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial DeviceCapabilities? Capabilities { get; set; }

        [ObservableProperty]
        public partial List<string> Flags { get; set; } = [];

        public RadioCapabilitiesViewModel(IAntRadio radioService)
        {
            Task.Run(async () =>
            {
                Capabilities = await radioService.GetDeviceCapabilities();
                Flags = Capabilities.GetType().GetProperties().
                    Where(t => t.PropertyType == typeof(bool) && (bool)t.GetValue(Capabilities)!).
                    ToList().
                    ConvertAll<string>(e => e.Name);
                Flags.Sort();
            });
        }
    }
}
