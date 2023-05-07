using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus.DeviceProfiles.MuscleOxygen;
using System;
using System.Linq;

namespace WpfUsbStickApp.ViewModels
{
    internal partial class MuscleOxygenViewModel : ObservableObject
    {
        private readonly MuscleOxygen muscleOxygen;
        public MuscleOxygen MuscleOxygen => muscleOxygen;
        public int[] Hours => Enumerable.Range(-15, 31).ToArray();
        public int[] Minutes { get; } = { 0, 15, 30, 45 };

        [ObservableProperty]
        private int hoursIndex = 15;
        [ObservableProperty]
        private int minutesIndex = 0;

        public MuscleOxygenViewModel(MuscleOxygen muscleOxygen)
        {
            this.muscleOxygen = muscleOxygen;
        }

        [RelayCommand]
        private void SetTime()
        {
            TimeSpan ts = new(Hours[HoursIndex], Minutes[MinutesIndex], 0);
            muscleOxygen.SendCommand(MuscleOxygen.CommandId.SetTime, ts, DateTime.UtcNow);
        }

        [RelayCommand]
        private void StartSession()
        {
            TimeSpan ts = new(Hours[HoursIndex], Minutes[MinutesIndex], 0);
            muscleOxygen.SendCommand(MuscleOxygen.CommandId.StartSession, ts, DateTime.UtcNow);
        }

        [RelayCommand]
        private void StopSession()
        {
            TimeSpan ts = new(Hours[HoursIndex], Minutes[MinutesIndex], 0);
            muscleOxygen.SendCommand(MuscleOxygen.CommandId.StopSession, ts, DateTime.UtcNow);
        }

        [RelayCommand]
        private void LogLap()
        {
            TimeSpan ts = new(Hours[HoursIndex], Minutes[MinutesIndex], 0);
            muscleOxygen.SendCommand(MuscleOxygen.CommandId.Lap, ts, DateTime.UtcNow);
        }
    }
}
