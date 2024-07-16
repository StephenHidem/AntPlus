using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus.DeviceProfiles;

namespace MauiAntGrpcClient.ViewModels
{
    [QueryProperty(nameof(MuscleOxygen), "Sensor")]
    public partial class MuscleOxygenViewModel : ObservableObject
    {
        private bool started = false;

        [ObservableProperty]
        private MuscleOxygen? muscleOxygen;
        [ObservableProperty]
        private int hours;
        [ObservableProperty]
        private int minutes;

        public static int[] HoursSource => Enumerable.Range(-15, 31).ToArray();
        public static int[] MinutesSource => [0, 15, 30, 45];

        [RelayCommand]
        private async Task SetTime()
        {
            TimeSpan ts = new(Hours, Minutes, 0);
            _ = await MuscleOxygen!.SendCommand(MuscleOxygen.CommandId.SetTime, ts, DateTime.UtcNow);
        }

        [RelayCommand(CanExecute = nameof(CanStartSession))]
        private async Task StartSession()
        {
            started = true;
            CheckCanExecutes();
            TimeSpan ts = new(Hours, Minutes, 0);
            _ = await MuscleOxygen!.SendCommand(MuscleOxygen.CommandId.StartSession, ts, DateTime.UtcNow);
        }
        private bool CanStartSession() => !started;

        [RelayCommand(CanExecute = nameof(CanStopSession))]
        private async Task StopSession()
        {
            started = false;
            CheckCanExecutes();
            TimeSpan ts = new(Hours, Minutes, 0);
            _ = await MuscleOxygen!.SendCommand(MuscleOxygen.CommandId.StopSession, ts, DateTime.UtcNow);
        }
        private bool CanStopSession() => started;

        [RelayCommand(CanExecute = nameof(CanLogLap))]
        private async Task LogLap()
        {
            TimeSpan ts = new(Hours, Minutes, 0);
            _ = await MuscleOxygen!.SendCommand(MuscleOxygen.CommandId.Lap, ts, DateTime.UtcNow);
        }
        private bool CanLogLap() => started;

        private void CheckCanExecutes()
        {
            StartSessionCommand.NotifyCanExecuteChanged();
            StopSessionCommand.NotifyCanExecuteChanged();
            LogLapCommand.NotifyCanExecuteChanged();
        }
    }
}
