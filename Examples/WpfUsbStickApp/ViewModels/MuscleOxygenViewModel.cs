using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WpfUsbStickApp.ViewModels
{
    internal partial class MuscleOxygenViewModel : ObservableObject
    {
        private bool started = false;

        public MuscleOxygen MuscleOxygen { get; }
        public CommonDataPages CommonDataPages => MuscleOxygen.CommonDataPages;

        [ObservableProperty]
        private int hours;
        [ObservableProperty]
        private int minutes;

        public static int[] HoursSource => Enumerable.Range(-15, 31).ToArray();
        public static int[] MinutesSource => new int[] { 0, 15, 30, 45 };

        public MuscleOxygenViewModel(MuscleOxygen muscleOxygen)
        {
            MuscleOxygen = muscleOxygen;
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
