using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles;
using System;
using System.Linq;

namespace WpfUsbStickApp.ViewModels
{
    internal partial class MuscleOxygenViewModel : ObservableObject
    {
        public MuscleOxygen MuscleOxygen { get; }

        public CommonDataPages CommonDataPages => MuscleOxygen.CommonDataPages;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(StartSessionCommand), nameof(StopSessionCommand), nameof(LogLapCommand))]
        private bool started;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(StartSessionCommand), nameof(StopSessionCommand), nameof(LogLapCommand))]
        private bool stopped = true;

        public int[] HoursSource => Enumerable.Range(-15, 31).ToArray();
        public int[] MinutesSource { get; } = { 0, 15, 30, 45 };

        [ObservableProperty]
        private int hours;
        [ObservableProperty]
        private int minutes;

        public MuscleOxygenViewModel(MuscleOxygen muscleOxygen)
        {
            MuscleOxygen = muscleOxygen;
        }

        [RelayCommand]
        private void SetTime()
        {
            TimeSpan ts = new(Hours, Minutes, 0);
            MuscleOxygen.SendCommand(MuscleOxygen.CommandId.SetTime, ts, DateTime.UtcNow);
        }

        [RelayCommand(CanExecute = nameof(CanStartSession))]
        private void StartSession()
        {
            Started = true;
            Stopped = false;
            TimeSpan ts = new(Hours, Minutes, 0);
            MuscleOxygen.SendCommand(MuscleOxygen.CommandId.StartSession, ts, DateTime.UtcNow);
        }

        private bool CanStartSession()
        {
            return !Started;
        }

        [RelayCommand(CanExecute = nameof(CanStopSession))]
        private void StopSession()
        {
            Started = false;
            Stopped = true;
            TimeSpan ts = new(Hours, Minutes, 0);
            MuscleOxygen.SendCommand(MuscleOxygen.CommandId.StopSession, ts, DateTime.UtcNow);
        }

        private bool CanStopSession()
        {
            return !Stopped;
        }

        [RelayCommand(CanExecute = nameof(CanLogLap))]
        private void LogLap()
        {
            TimeSpan ts = new(Hours, Minutes, 0);
            MuscleOxygen.SendCommand(MuscleOxygen.CommandId.Lap, ts, DateTime.UtcNow);
        }

        private bool CanLogLap()
        {
            return Started;
        }
    }
}
