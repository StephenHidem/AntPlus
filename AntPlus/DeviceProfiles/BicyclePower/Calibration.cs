using System;
using System.ComponentModel;
using System.Linq;

namespace AntPlus.DeviceProfiles.BicyclePower
{
    public class Calibration : INotifyPropertyChanged
    {
        private readonly BicyclePower bp;

        public event PropertyChangedEventHandler PropertyChanged;

        private enum CalibrationRequestId
        {
            ManualZero = 0xAA,
            AutoZeroConfiguration = 0xAB,
            CustomCalibration = 0xBA,
            CustomCalibrationUpdate = 0xBC,
        }

        private enum CalibrationResponseId
        {
            CTFDefinedMsg = 0x10,
            AutoZeroSupport = 0x12,
            Success = 0xAC,
            Failed = 0xAF,
            CustomCalibration = 0xBB,
            CustomCalibrationUpdate = 0xBD,
        }

        public enum AutoZero
        {
            Off = 0,
            On = 1,
            NotSupported = 0xFF
        }

        public bool Succeeded { get; private set; }
        public AutoZero AutoZeroStatus { get; private set; }
        public short CalibrationData { get; private set; }
        public bool AutoZeroSupported { get; private set; }
        public byte[] CustomCalibrationParameters { get; private set; }

        public Calibration(BicyclePower bp)
        {
            this.bp = bp;
        }

        public void Parse(byte[] dataPage)
        {
            switch ((CalibrationResponseId)dataPage[1])
            {
                case CalibrationResponseId.CTFDefinedMsg:
                    bp.CTFSensor.ParseCalibrationMessage(dataPage);
                    break;
                case CalibrationResponseId.AutoZeroSupport:
                    AutoZeroSupported = (dataPage[2] & 0x01) == 0x01;
                    AutoZeroStatus = (dataPage[2] & 0x02) == 0x02 ? AutoZero.On : AutoZero.Off;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutoZeroSupported)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutoZeroStatus)));
                    break;
                case CalibrationResponseId.Success:
                    Succeeded = true;
                    AutoZeroStatus = (AutoZero)dataPage[2];
                    CalibrationData = BitConverter.ToInt16(dataPage, 6);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CalibrationData)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutoZeroStatus)));
                    break;
                case CalibrationResponseId.Failed:
                    Succeeded = false;
                    AutoZeroStatus = (AutoZero)dataPage[2];
                    CalibrationData = BitConverter.ToInt16(dataPage, 6);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CalibrationData)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutoZeroStatus)));
                    break;
                case CalibrationResponseId.CustomCalibration:
                    Succeeded = true;
                    CustomCalibrationParameters = dataPage.Skip(2).ToArray();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CustomCalibrationParameters)));
                    break;
                case CalibrationResponseId.CustomCalibrationUpdate:
                    Succeeded = true;
                    CustomCalibrationParameters = dataPage.Skip(2).ToArray();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CustomCalibrationParameters)));
                    break;
                default:
                    break;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Succeeded)));
        }

        public void RequestManualCalibration()
        {
            bp.SendExtAcknowledgedMessage(new byte[] { (byte)DataPage.Calibration, (byte)CalibrationRequestId.ManualZero, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        }

        public void SetAutoZeroConfiguration(AutoZero autoZero)
        {
            bp.SendExtAcknowledgedMessage(new byte[] { (byte)DataPage.Calibration, (byte)CalibrationRequestId.AutoZeroConfiguration, (byte)autoZero, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        }

        public void RequestCustomParameters()
        {
            bp.SendExtAcknowledgedMessage(new byte[] { (byte)DataPage.Calibration, (byte)CalibrationRequestId.CustomCalibration, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        }

        public void SetCustomParameters(byte[] customParameters)
        {
            byte[] msg = new byte[] { (byte)DataPage.Calibration, (byte)CalibrationRequestId.CustomCalibrationUpdate };
            msg = msg.Concat(customParameters).ToArray();
            bp.SendExtAcknowledgedMessage(msg);
        }
    }
}
