using System;
using System.Linq;

namespace DeviceProfiles
{
    public partial class BicyclePower
    {
        public class BicycleCalibrationData
        {
            private BicyclePower bp;

            private enum CalibrationRequestId
            {
                ManualZero = 0xAA,
                AutoZeroConfiguration = 0xAB,
                CustomCalibration = 0xBA,
                CustomCalibrationUpdate = 0xBC,
            }

            private enum CalibrationResponseId
            {
                AutoZeroStatus = 0x12,
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
            public bool AutoZeroEnable { get; private set; }
            public byte[] CustomCalibrationParameters { get; private set; }

            public BicycleCalibrationData(BicyclePower bp)
            {
                this.bp = bp;
            }

            public void Parse(byte[] page)
            {
                switch ((CalibrationResponseId)page[1])
                {
                    case CalibrationResponseId.AutoZeroStatus:
                        AutoZeroEnable = (page[2] & 0x01) == 0x01;
                        AutoZeroStatus = (page[2] & 0x02) == 0x02 ? AutoZero.On : AutoZero.Off;
                        break;
                    case CalibrationResponseId.Success:
                        Succeeded = true;
                        AutoZeroStatus = (AutoZero)page[2];
                        CalibrationData = BitConverter.ToInt16(page, 6);
                        break;
                    case CalibrationResponseId.Failed:
                        Succeeded = false;
                        AutoZeroStatus = (AutoZero)page[2];
                        CalibrationData = BitConverter.ToInt16(page, 6);
                        break;
                    case CalibrationResponseId.CustomCalibration:
                        Succeeded = true;
                        CustomCalibrationParameters = page.Skip(2).ToArray();
                        break;
                    case CalibrationResponseId.CustomCalibrationUpdate:
                        Succeeded = true;
                        CustomCalibrationParameters = page.Skip(2).ToArray();
                        break;
                    default:
                        break;
                }
            }

            public void SetAutoZeroConfiguration(AutoZero autoZero)
            {
                bp.SendExtAcknowledgedMessage(new byte[] { (byte)DataPage.Calibration, (byte)CalibrationRequestId.AutoZeroConfiguration, (byte)autoZero, 0, 0, 0, 0, 0 });
            }
        }
    }
}
