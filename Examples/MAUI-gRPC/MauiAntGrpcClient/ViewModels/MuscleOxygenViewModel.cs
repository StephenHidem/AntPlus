using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntRadioInterface;

namespace MauiAntGrpcClient.ViewModels
{
    public partial class MuscleOxygenViewModel : ObservableObject, IQueryAttributable
    {
        private readonly ILogger<MuscleOxygenViewModel> _logger;
        private bool started = false;

        [ObservableProperty]
        private MuscleOxygen muscleOxygen = null!;
        [ObservableProperty]
        private int hours;
        [ObservableProperty]
        private int minutes;

        public static int[] HoursSource => Enumerable.Range(-15, 31).ToArray();
        public static int[] MinutesSource => [0, 15, 30, 45];

        public MuscleOxygenViewModel(ILogger<MuscleOxygenViewModel> logger)
        {
            _logger = logger;
            _logger.LogInformation("Created MuscleOxygenViewModel");
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            _logger.LogInformation($"{nameof(ApplyQueryAttributes)}");
            MuscleOxygen = (MuscleOxygen)query["Sensor"];
        }

        [RelayCommand]
        private async Task<MessagingReturnCode> SetTime()
        {
            TimeSpan ts = new(Hours, Minutes, 0);
            return await MuscleOxygen.SendCommand(MuscleOxygen.CommandId.SetTime, ts, DateTime.UtcNow);
        }

        [RelayCommand(CanExecute = nameof(CanStartSession))]
        private async Task<MessagingReturnCode> StartSession()
        {
            started = true;
            CheckCanExecutes();
            TimeSpan ts = new(Hours, Minutes, 0);
            return await MuscleOxygen.SendCommand(MuscleOxygen.CommandId.StartSession, ts, DateTime.UtcNow);
        }

        private bool CanStartSession()
        {
            return !started;
        }

        [RelayCommand(CanExecute = nameof(CanStopSession))]
        private async Task<MessagingReturnCode> StopSession()
        {
            started = false;
            CheckCanExecutes();
            TimeSpan ts = new(Hours, Minutes, 0);
            return await MuscleOxygen.SendCommand(MuscleOxygen.CommandId.StopSession, ts, DateTime.UtcNow);
        }

        private bool CanStopSession()
        {
            return started;
        }

        [RelayCommand(CanExecute = nameof(CanLogLap))]
        private async Task<MessagingReturnCode> LogLap()
        {
            TimeSpan ts = new(Hours, Minutes, 0);
            return await MuscleOxygen.SendCommand(MuscleOxygen.CommandId.Lap, ts, DateTime.UtcNow);
        }

        private bool CanLogLap()
        {
            return started;
        }

        private void CheckCanExecutes()
        {
            StartSessionCommand.NotifyCanExecuteChanged();
            StopSessionCommand.NotifyCanExecuteChanged();
            LogLapCommand.NotifyCanExecuteChanged();
        }
    }
}
