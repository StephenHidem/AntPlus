﻿using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// Bicycle power calibration class.
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public class Calibration : INotifyPropertyChanged
    {
        private readonly Bicycle _bicycle;
        private readonly ILogger _logger;

        /// <summary>Occurs when a property value changes.</summary>
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

        /// <summary>Gets the calibration status.</summary>
        public CalibrationResponse CalibrationStatus { get; private set; }
        /// <summary>Gets the automatic zero status.</summary>
        public AutoZero AutoZeroStatus { get; private set; }
        /// <summary>Gets the manufacturer specific calibration data.</summary>
        public short CalibrationData { get; private set; }
        /// <summary>Gets a value indicating whether automatic zero is supported.</summary>
        public bool AutoZeroSupported { get; private set; }
        /// <summary>Gets the custom calibration parameters.</summary>
        public byte[] CustomCalibrationParameters { get; private set; }
        /// <summary>Gets the reported measurements collection. There may be one or more measurement data types reported.</summary>
        public ObservableCollection<MeasurementOutputData> Measurements { get; private set; } = new ObservableCollection<MeasurementOutputData>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Calibration"/> class.
        /// </summary>
        /// <param name="bicycle">The <see cref="Bicycle"/>.</param>
        /// <param name="logger">Logger to use.</param>
        public Calibration(Bicycle bicycle, ILogger logger)
        {
            _bicycle = bicycle;
            _logger = logger;
        }

        /// <summary>Parses the specified data page.</summary>
        /// <param name="dataPage">The data page.</param>
        public void Parse(byte[] dataPage)
        {
            _logger.LogDebug("CalibrationResponseId: {Id}", (CalibrationResponseId)dataPage[1]);
            switch ((CalibrationResponseId)dataPage[1])
            {
                case CalibrationResponseId.CTFDefinedMsg:
                    _bicycle.CTFSensor.ParseCalibrationMessage(dataPage);
                    break;
                case CalibrationResponseId.AutoZeroSupport:
                    AutoZeroSupported = (dataPage[2] & 0x01) == 0x01;
                    AutoZeroStatus = (dataPage[2] & 0x02) == 0x02 ? AutoZero.On : AutoZero.Off;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutoZeroSupported)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutoZeroStatus)));
                    break;
                case CalibrationResponseId.Success:
                    CalibrationStatus = CalibrationResponse.Succeeded;
                    AutoZeroStatus = (AutoZero)dataPage[2];
                    CalibrationData = BitConverter.ToInt16(dataPage, 6);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CalibrationStatus)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CalibrationData)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutoZeroStatus)));
                    break;
                case CalibrationResponseId.Failed:
                    CalibrationStatus = CalibrationResponse.Failed;
                    AutoZeroStatus = (AutoZero)dataPage[2];
                    CalibrationData = BitConverter.ToInt16(dataPage, 6);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CalibrationStatus)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CalibrationData)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutoZeroStatus)));
                    break;
                case CalibrationResponseId.CustomCalibration:
                    CalibrationStatus = CalibrationResponse.Succeeded;
                    CustomCalibrationParameters = dataPage.Skip(2).ToArray();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CalibrationStatus)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CustomCalibrationParameters)));
                    break;
                case CalibrationResponseId.CustomCalibrationUpdate:
                    CalibrationStatus = CalibrationResponse.Succeeded;
                    CustomCalibrationParameters = dataPage.Skip(2).ToArray();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CalibrationStatus)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CustomCalibrationParameters)));
                    break;
                default:
                    _logger.LogWarning("Unknown CalibrationResponseId = {CalibrationResponseId}.", dataPage[1]);
                    break;
            }
        }

        internal void ParseMeasurementOutputData(byte[] dataPage)
        {
            MeasurementOutputData measurement = Measurements.FirstOrDefault(m => m.MeasurementType == (MeasurementOutputData.DataType)dataPage[2]);
            if (measurement == null)
            {
                measurement = new MeasurementOutputData(dataPage);
                lock (CollectionLock)
                {
                    Measurements.Add(measurement);
                }
            }
            measurement.Parse(dataPage);
        }

        /// <summary>Requests manual calibration.</summary>
        /// <returns><see cref="MessagingReturnCode"/></returns>
        public async Task<MessagingReturnCode> RequestManualCalibration()
        {
            CalibrationStatus = CalibrationResponse.InProgress;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CalibrationStatus)));
            return await _bicycle.SendExtAcknowledgedMessage(new byte[] { (byte)DataPage.Calibration, (byte)CalibrationRequestId.ManualZero, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        }

        /// <summary>Sets the sensor automatic zero configuration.</summary>
        /// <param name="autoZero">The automatic zero.</param>
        /// <returns><see cref="MessagingReturnCode"/></returns>
        public async Task<MessagingReturnCode> SetAutoZeroConfiguration(AutoZero autoZero)
        {
            CalibrationStatus = CalibrationResponse.Unknown;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CalibrationStatus)));
            return await _bicycle.SendExtAcknowledgedMessage(new byte[] { (byte)DataPage.Calibration, (byte)CalibrationRequestId.AutoZeroConfiguration, (byte)autoZero, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        }

        /// <summary>Requests the manufacturer specific custom calibration parameters.</summary>
        /// <returns><see cref="MessagingReturnCode"/></returns>
        public async Task<MessagingReturnCode> RequestCustomParameters()
        {
            CalibrationStatus = CalibrationResponse.InProgress;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CalibrationStatus)));
            return await _bicycle.SendExtAcknowledgedMessage(new byte[] { (byte)DataPage.Calibration, (byte)CalibrationRequestId.CustomCalibration, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CalibrationStatus)));
            byte[] msg = new byte[] { (byte)DataPage.Calibration, (byte)CalibrationRequestId.CustomCalibrationUpdate };
            msg = msg.Concat(customParameters).ToArray();
            return await _bicycle.SendExtAcknowledgedMessage(msg);
        }
    }
}
