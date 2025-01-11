using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus.DeviceProfiles;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace WpfUsbStickApp.ViewModels
{
    internal partial class BikeRadarViewModel
    {
        private bool _shutdown;

        public BikeRadar BikeRadar { get; }

        public BikeRadarViewModel(BikeRadar bikeRadar)
        {
            BikeRadar = bikeRadar;
            BikeRadar.PropertyChanged += BikeRadar_PropertyChanged;
        }

        private void BikeRadar_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // notify UI if state changed or device went offline
            if (e.PropertyName == nameof(BikeRadar.State) || e.PropertyName == nameof(BikeRadar.Offline))
            {
                _ = Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ShutdownCommand.NotifyCanExecuteChanged();
                    AbortShutdownCommand.NotifyCanExecuteChanged();
                });
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteShutdown))]
        public async Task Shutdown()
        {
            _shutdown = true;
            ShutdownCommand.NotifyCanExecuteChanged();
            AbortShutdownCommand.NotifyCanExecuteChanged();
            _ = await BikeRadar.Shutdown(BikeRadar.Command.Shutdown);
        }

        private bool CanExecuteShutdown() => !_shutdown && !BikeRadar.Offline
            && BikeRadar.State == BikeRadar.DeviceState.Broadcasting
            || BikeRadar.State == BikeRadar.DeviceState.ShutdownAborted;

        [RelayCommand(CanExecute = nameof(CanExecuteAbortShutdown))]
        public async Task AbortShutdown()
        {
            _shutdown = false;
            ShutdownCommand.NotifyCanExecuteChanged();
            AbortShutdownCommand.NotifyCanExecuteChanged();
            _ = await BikeRadar.Shutdown(BikeRadar.Command.AbortShutdown);
        }

        private bool CanExecuteAbortShutdown() => _shutdown && !BikeRadar.Offline
            && BikeRadar.State == BikeRadar.DeviceState.ShutdownRequested;
    }
}
