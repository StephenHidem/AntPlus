using AntPlusUsbClient.Views;
using SmallEarthTech.AntPlus.DeviceProfiles.MuscleOxygen;
using System;
using System.Linq;
using System.Windows.Input;

namespace AntPlusUsbClient.ViewModels
{
    internal class MuscleOxygenViewModel
    {
        private readonly MuscleOxygen muscleOxygen;

        public MuscleOxygen MuscleOxygen => muscleOxygen;

        public int[] HoursOffset => Enumerable.Range(-15, 31).ToArray();
        public int[] MinuteOffset { get; } = { 0, 15, 30, 45 };

        public RoutedCommand SetTime { get; private set; } = new RoutedCommand();
        public RoutedCommand StartSession { get; private set; } = new RoutedCommand();
        public RoutedCommand StopSession { get; private set; } = new RoutedCommand();
        public RoutedCommand Lap { get; private set; } = new RoutedCommand();

        public CommandBinding[] CommandBindings { get; private set; }

        public MuscleOxygenViewModel(MuscleOxygen muscleOxygen)
        {
            this.muscleOxygen = muscleOxygen;

            CommandBindings = new CommandBinding[] {
                new CommandBinding(SetTime, SetTimeExecuted),
                new CommandBinding(StartSession, StartSessionExecuted),
                new CommandBinding(StopSession, StopSessionParametersExecuted),
                new CommandBinding(Lap, LapExecuted),
            };
        }

        private void SetTimeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MuscleOxygenWindow mw = sender as MuscleOxygenWindow;
            TimeSpan ts = new TimeSpan((int)mw.HourOffset.SelectedValue, (int)mw.MinuteOffset.SelectedValue, 0);
            muscleOxygen.SendCommand(MuscleOxygen.CommandId.SetTime, ts, DateTime.UtcNow);
        }

        private void StartSessionExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MuscleOxygenWindow mw = sender as MuscleOxygenWindow;
            TimeSpan ts = new TimeSpan((int)mw.HourOffset.SelectedValue, (int)mw.MinuteOffset.SelectedValue, 0);
            muscleOxygen.SendCommand(MuscleOxygen.CommandId.StartSession, ts, DateTime.UtcNow);
        }

        private void StopSessionParametersExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MuscleOxygenWindow mw = sender as MuscleOxygenWindow;
            TimeSpan ts = new TimeSpan((int)mw.HourOffset.SelectedValue, (int)mw.MinuteOffset.SelectedValue, 0);
            muscleOxygen.SendCommand(MuscleOxygen.CommandId.StopSession, ts, DateTime.UtcNow);
        }

        private void LapExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MuscleOxygenWindow mw = sender as MuscleOxygenWindow;
            TimeSpan ts = new TimeSpan((int)mw.HourOffset.SelectedValue, (int)mw.MinuteOffset.SelectedValue, 0);
            muscleOxygen.SendCommand(MuscleOxygen.CommandId.Lap, ts, DateTime.UtcNow);
        }
    }
}
