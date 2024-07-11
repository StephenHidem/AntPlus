using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower
{
    enum CalibrationRequestId
    {
        ManualZero = 0xAA,
        AutoZeroConfiguration = 0xAB,
        CustomCalibration = 0xBA,
        CustomCalibrationUpdate = 0xBC,
    }

    enum CalibrationResponseId
    {
        CTFDefinedMsg = 0x10,
        AutoZeroSupport = 0x12,
        Success = 0xAC,
        Failed = 0xAF,
        CustomCalibration = 0xBB,
        CustomCalibrationUpdate = 0xBD,
    }

    /// <summary>Response to a calibration operation.</summary>
    public enum CalibrationResponse
    {
        /// <summary>Unknown</summary>
        Unknown,
        /// <summary>In progress</summary>
        InProgress,
        /// <summary>Succeeded</summary>
        Succeeded,
        /// <summary>Failed</summary>
        Failed,
    }

    /// <summary>Auto zero status.</summary>
    public enum AutoZero
    {
        /// <summary>Off</summary>
        Off = 0,
        /// <summary>On</summary>
        On = 1,
        /// <summary>Not supported</summary>
        NotSupported = 0xFF
    }

    public partial class StandardPowerSensor
    {
        /// <summary>
        /// The collection lock.
        /// </summary>
        /// <remarks>
        /// An application should use the collection lock to ensure thread safe access to the
        /// collection. For example, the code behind for a WPF window should include -
        /// <code>BindingOperations.EnableCollectionSynchronization(bicyclePower.Calibration.Measurements, bicyclePower.Calibration.CollectionLock);</code>
        /// This ensures changes to the collection are thread safe and marshalled on the UI thread.
        /// </remarks>
        public object CollectionLock = new object();
        /// <summary>Gets a value indicating whether automatic zero is supported.</summary>
        [ObservableProperty]
        private bool autoZeroSupported;
        /// <summary>Gets the automatic zero status.</summary>
        [ObservableProperty]
        private AutoZero autoZeroStatus;
        /// <summary>Gets the manufacturer specific calibration data.</summary>
        [ObservableProperty]
        private short calibrationData;
        /// <summary>Gets the custom calibration parameters.</summary>
        [ObservableProperty]
        private byte[] customCalibrationParameters;
        /// <summary>Gets the reported measurements collection. There may be one or more measurement data types reported.</summary>
        public ObservableCollection<MeasurementOutputData> Measurements { get; private set; } = new ObservableCollection<MeasurementOutputData>();

        private void ParseCalibrationPage(byte[] page)
        {
            logger.LogDebug("CalibrationResponseId: {Id}", (CalibrationResponseId)page[1]);
            switch ((CalibrationResponseId)page[1])
            {
                case CalibrationResponseId.AutoZeroSupport:
                    AutoZeroSupported = (page[2] & 0x01) == 0x01;
                    AutoZeroStatus = (page[2] & 0x02) == 0x02 ? AutoZero.On : AutoZero.Off;
                    break;
                case CalibrationResponseId.Success:
                    CalibrationStatus = CalibrationResponse.Succeeded;
                    AutoZeroStatus = (AutoZero)page[2];
                    CalibrationData = BitConverter.ToInt16(page, 6);
                    break;
                case CalibrationResponseId.Failed:
                    CalibrationStatus = CalibrationResponse.Failed;
                    AutoZeroStatus = (AutoZero)page[2];
                    CalibrationData = BitConverter.ToInt16(page, 6);
                    break;
                case CalibrationResponseId.CustomCalibration:
                case CalibrationResponseId.CustomCalibrationUpdate:
                    CalibrationStatus = CalibrationResponse.Succeeded;
                    CustomCalibrationParameters = page.Skip(2).ToArray();
                    break;
                default:
                    logger.LogWarning("Unknown CalibrationResponseId = {CalibrationResponseId}.", page[1]);
                    break;
            }
        }

        /// <summary>Sets the sensor automatic zero configuration.</summary>
        /// <param name="autoZero">The automatic zero.</param>
        /// <returns><see cref="MessagingReturnCode"/></returns>
        public async Task<MessagingReturnCode> SetAutoZeroConfiguration(AutoZero autoZero)
        {
            CalibrationStatus = CalibrationResponse.Unknown;
            return await SendExtAcknowledgedMessage(new byte[] { (byte)DataPage.Calibration, (byte)CalibrationRequestId.AutoZeroConfiguration, (byte)autoZero, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        }

        /// <summary>Requests the manufacturer specific custom calibration parameters.</summary>
        /// <returns><see cref="MessagingReturnCode"/></returns>
        public async Task<MessagingReturnCode> RequestCustomParameters()
        {
            CalibrationStatus = CalibrationResponse.InProgress;
            return await SendExtAcknowledgedMessage(new byte[] { (byte)DataPage.Calibration, (byte)CalibrationRequestId.CustomCalibration, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        }

        /// <summary>Sets the custom calibration parameters. This is manufacturer specified limited to a
        /// maximum of 6 bytes.</summary>
        /// <param name="customParameters">The custom parameters. Defined by the manufacturer.</param>
        /// <returns><see cref="MessagingReturnCode"/></returns>
        /// <exception cref="System.ArgumentException">Custom parameters must be 6 bytes in length.</exception>
        public async Task<MessagingReturnCode> SetCustomParameters(byte[] customParameters)
        {
            if (customParameters.Length != 6)
            {
                throw new ArgumentException("Custom parameters must be 6 bytes in length.");
            }
            CalibrationStatus = CalibrationResponse.InProgress;
            byte[] msg = new byte[] { (byte)DataPage.Calibration, (byte)CalibrationRequestId.CustomCalibrationUpdate };
            msg = msg.Concat(customParameters).ToArray();
            return await SendExtAcknowledgedMessage(msg);
        }

        internal void ParseMeasurementOutputData(byte[] page)
        {
            MeasurementOutputData measurement = Measurements.FirstOrDefault(m => m.MeasurementType == (MeasurementType)page[2]);
            if (measurement == null)
            {
                measurement = new MeasurementOutputData(page);
                lock (CollectionLock)
                {
                    Measurements.Add(measurement);
                }
            }
            measurement.Parse(page);
        }
    }
}
