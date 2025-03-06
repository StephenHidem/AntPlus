using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus.DeviceProfiles;
using System.ComponentModel;

namespace MauiAntGrpcClient.ViewModels
{
    public partial class BikeRadarViewModel : ObservableObject, IQueryAttributable
    {
        private bool _shutdown;

        [ObservableProperty]
        public partial BikeRadar? BikeRadar { get; set; }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            BikeRadar = (BikeRadar)query["Sensor"];
            BikeRadar.PropertyChanged += BikeRadar_PropertyChanged;
        }

        private void BikeRadar_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // notify UI if state changed or device went offline
            if (e.PropertyName == nameof(BikeRadar.State) || e.PropertyName == nameof(BikeRadar.Offline))
            {
                MainThread.BeginInvokeOnMainThread(() =>
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
            _ = await BikeRadar!.Shutdown(BikeRadar.Command.Shutdown);
        }

        private bool CanExecuteShutdown() => !_shutdown && BikeRadar != null && !BikeRadar.Offline
            && (BikeRadar.State == BikeRadar.DeviceState.Broadcasting
            || BikeRadar.State == BikeRadar.DeviceState.ShutdownAborted);

        [RelayCommand(CanExecute = nameof(CanExecuteAbortShutdown))]
        public async Task AbortShutdown()
        {
            _shutdown = false;
            ShutdownCommand.NotifyCanExecuteChanged();
            AbortShutdownCommand.NotifyCanExecuteChanged();
            _ = await BikeRadar!.Shutdown(BikeRadar.Command.AbortShutdown);
        }

        private bool CanExecuteAbortShutdown() => _shutdown && BikeRadar != null && !BikeRadar.Offline
            && BikeRadar.State == BikeRadar.DeviceState.ShutdownRequested;
    }
}
