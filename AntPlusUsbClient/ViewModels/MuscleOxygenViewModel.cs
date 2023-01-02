using AntPlus.DeviceProfiles;
using AntPlusUsbClient.Views;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using static AntPlus.DeviceProfiles.MuscleOxygen;

namespace AntPlusUsbClient.ViewModels
{
    internal class MuscleOxygenViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly MuscleOxygen muscleOxygen;

        public MuscleOxygen MuscleOxygen => muscleOxygen;

        public int[] HoursOffset => Enumerable.Range(-15, 31).ToArray();
        public int[] MinuteOffset { get; } = { 0, 15, 30, 45 };
        //public byte EventCount => muscleOxygen.EventCount;
        //public bool UtcTimeRequired => muscleOxygen.UtcTimeRequired;
        //public bool SupportsAntFs => muscleOxygen.SupportsAntFs;
        //public MeasurementInterval Interval => muscleOxygen.Interval;
        //public TotalHemoglobin TotalHemoglobinConcentration => muscleOxygen.TotalHemoglobinConcentration;
        //public SaturatedHemoglobin PreviousSaturatedHemoglobin => muscleOxygen.PreviousSaturatedHemoglobin;
        //public SaturatedHemoglobin CurrentSaturatedHemoglobin => muscleOxygen.CurrentSaturatedHemoglobin;

        public RoutedCommand SetTime { get; private set; } = new RoutedCommand();
        public RoutedCommand StartSession { get; private set; } = new RoutedCommand();
        public RoutedCommand StopSession { get; private set; } = new RoutedCommand();
        public RoutedCommand Lap { get; private set; } = new RoutedCommand();

        public CommandBinding[] CommandBindings { get; private set; }

        public MuscleOxygenViewModel(MuscleOxygen muscleOxygen)
        {
            this.muscleOxygen = muscleOxygen;

            CommandBindings = new CommandBinding[] {
                new CommandBinding(SetTime, SetTimeExecuted, SetTimeCanExecute),
                new CommandBinding(StartSession, StartSessionExecuted, StartSessionCanExecute),
                new CommandBinding(StopSession, StopSessionParametersExecuted, StopSessionCanExecute),
                new CommandBinding(Lap, LapExecuted, LapCanExecute),
            };
        }

        private void SetTimeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MuscleOxygenWindow mw = sender as MuscleOxygenWindow;
            TimeSpan ts = new TimeSpan((int)mw.HourOffset.SelectedValue, (int)mw.MinuteOffset.SelectedValue, 0);
            muscleOxygen.SendCommand(CommandId.SetTime, ts, DateTime.UtcNow);
        }

        private void SetTimeCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void StartSessionExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MuscleOxygenWindow mw = sender as MuscleOxygenWindow;
            TimeSpan ts = new TimeSpan((int)mw.HourOffset.SelectedValue, (int)mw.MinuteOffset.SelectedValue, 0);
            muscleOxygen.SendCommand(CommandId.StartSession, ts, DateTime.UtcNow);
        }

        private void StartSessionCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void StopSessionParametersExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MuscleOxygenWindow mw = sender as MuscleOxygenWindow;
            TimeSpan ts = new TimeSpan((int)mw.HourOffset.SelectedValue, (int)mw.MinuteOffset.SelectedValue, 0);
            muscleOxygen.SendCommand(CommandId.StopSession, ts, DateTime.UtcNow);
        }

        private void StopSessionCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void LapExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MuscleOxygenWindow mw = sender as MuscleOxygenWindow;
            TimeSpan ts = new TimeSpan((int)mw.HourOffset.SelectedValue, (int)mw.MinuteOffset.SelectedValue, 0);
            muscleOxygen.SendCommand(CommandId.Lap, ts, DateTime.UtcNow);
        }

        private void LapCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
