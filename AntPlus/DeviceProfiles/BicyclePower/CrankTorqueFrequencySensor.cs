using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// The crank torque frequency sensor class.
    /// </summary>
    public partial class CrankTorqueFrequencySensor : BicyclePower
    {
        public const int CalEventId = 3001;
        public const int CtfEventId = 3002;
        public const int AckCtfIdEventId = 3003;

        /// <inheritdoc/>
        public override Stream DeviceImageStream => typeof(CrankTorqueFrequencySensor).Assembly.GetManifestResourceStream("SmallEarthTech.AntPlus.Images.CrankTorqueFrequency.png");
        /// <inheritdoc/>
        public override string ToString() => "Bike Power (CTF)";

        /// <summary>Crank torque frequency defined IDs. These are used by methods that save the slope or serial number to the sensor flash or when the zero offset is reported by the sensor.</summary>
        public enum CTFDefinedId
        {
            /// <summary>The zero offset ID.</summary>
            ZeroOffset = 1,
            /// <summary>The slope ID.</summary>
            Slope = 2,
            /// <summary>The serial number ID.</summary>
            SerialNumber = 3,
            /// <summary>The acknowledgement ID.</summary>
            Ack = 0xAC
        }

        private bool isFirstPage = true;
        private byte prevUpdateEventCount;
        private ushort prevTimeStamp;
        private ushort prevTorqueTicks;

        /// <summary>Gets the zero offset in Hz.</summary>
        /// <remarks>Offset is only received from the sensor when a calibration is requested or bike has been coasting.</remarks>
        [ObservableProperty]
        private ushort offset;
        /// <summary>Gets the slope. Slope ranges in value from 10.0Nm/Hz to 50.0Nm/Hz. Resolution is 0.1 Nm/Hz.</summary>
        [ObservableProperty]
        private double slope;
        /// <summary>Gets the cadence in revolutions per minute.</summary>
        [ObservableProperty]
        private double cadence;
        /// <summary>Gets the torque in Nm.</summary>
        [ObservableProperty]
        private double torque;
        /// <summary>Gets the power in watts.</summary>
        [ObservableProperty]
        private double power;

        /// <summary>Initializes a new instance of the <see cref="CrankTorqueFrequencySensor" /> class.</summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, int)"/>
        public CrankTorqueFrequencySensor(ChannelId channelId, IAntChannel antChannel, ILogger<CrankTorqueFrequencySensor> logger, int timeout)
            : base(channelId, antChannel, logger, timeout)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="CrankTorqueFrequencySensor" /> class.</summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, TimeoutOptions?)"/>
        public CrankTorqueFrequencySensor(ChannelId channelId, IAntChannel antChannel, ILogger<CrankTorqueFrequencySensor> logger, TimeoutOptions? timeoutOptions)
            : base(channelId, antChannel, logger, timeoutOptions)
        {
        }

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);
            switch ((DataPage)dataPage[0])
            {
                case DataPage.Calibration:
                    ParseCalibrationMessage(dataPage);
                    break;
                case DataPage.CrankTorqueFrequency:
                    ParseCTFMessage(dataPage);
                    break;
                default:
                    _logger.UnknownDataPage(dataPage);
                    break;
            }
        }

        private void ParseCTFMessage(byte[] dataPage)
        {
            // clear calibration pending
            if (CalibrationStatus == CalibrationResponse.InProgress)
            {
                CalibrationStatus = CalibrationResponse.Succeeded;
            }

            byte updateEventCount = dataPage[1];

            // the data is in big endian order; this also flips the order of the fields
            byte[] data = dataPage.Skip(2).Reverse().ToArray();
            ushort torqueTicks = BitConverter.ToUInt16(data, 0);
            ushort timeStamp = BitConverter.ToUInt16(data, 2);
            Slope = BitConverter.ToUInt16(data, 4) / 10.0;

            if (isFirstPage)
            {
                isFirstPage = false;
                prevUpdateEventCount = updateEventCount;
                prevTimeStamp = timeStamp;
                prevTorqueTicks = torqueTicks;
                return;
            }

            if (prevUpdateEventCount != updateEventCount && prevTorqueTicks != torqueTicks)
            {
                // calculations
                double elapsedTime = Utils.CalculateDelta(timeStamp, ref prevTimeStamp) * 0.0005;
                double cadencePeriod = elapsedTime / Utils.CalculateDelta(updateEventCount, ref prevUpdateEventCount);
                Cadence = 60.0 / cadencePeriod;
                double torqueFreq = (1.0 / (elapsedTime / Utils.CalculateDelta(torqueTicks, ref prevTorqueTicks))) - Offset;
                Torque = torqueFreq / Slope;
                Power = Torque * Cadence * Math.PI / 30.0;
            }
        }

        private void ParseCalibrationMessage(byte[] dataPage)
        {
            // check if this is CTF calibration message
            if (dataPage[1] == 0x10)
            {
                switch ((CTFDefinedId)dataPage[2])
                {
                    case CTFDefinedId.ZeroOffset:
                        Offset = BitConverter.ToUInt16(dataPage.Skip(6).Reverse().ToArray(), 0);
                        break;
                    case CTFDefinedId.Ack:
                        CalibrationStatus = CalibrationResponse.Succeeded;
                        switch ((CTFDefinedId)dataPage[3])
                        {
                            case CTFDefinedId.Slope:
                                _logger.LogInformation("Slope saved to flash.");
                                break;
                            case CTFDefinedId.SerialNumber:
                                _logger.LogInformation("Serial number saved to flash.");
                                break;
                            default:
                                _logger.UnknownDataPage(dataPage, AckCtfIdEventId);
                                break;
                        }
                        break;
                    default:
                        _logger.UnknownDataPage(dataPage, CtfEventId);
                        break;
                }
            }
            else
            {
                _logger.UnknownDataPage(dataPage, CalEventId);
            }
        }

        /// <summary>Saves the slope to flash.</summary>
        /// <param name="slope">The slope. Valid range is 10.0 to 50.0 Nm/Hz. Resolution is 0.1 Nm/Hz.</param>
        /// <returns>Status of the request.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Invalid slope value.</exception>
        public async Task<MessagingReturnCode> SaveSlopeToFlash(double slope)
        {
            // check range
            if (slope < 10.0 || slope > 50.0)
            {
                throw new ArgumentOutOfRangeException(nameof(slope));
            }

            CalibrationStatus = CalibrationResponse.InProgress;
            ushort slp = Convert.ToUInt16(slope * 10.0);    // scale by resolution

            byte[] msg = new byte[] { (byte)DataPage.Calibration, 0x10, (byte)CTFDefinedId.Slope, 0xFF, 0xFF, 0xFF };
            msg = msg.Concat(BitConverter.GetBytes(slp).Reverse()).ToArray();
            return await SendExtAcknowledgedMessage(msg);
        }

        /// <summary>Saves the serial number to flash.</summary>
        /// <param name="serialNumber">The serial number.</param>
        /// <returns>Status of the request.</returns>
        public async Task<MessagingReturnCode> SaveSerialNumberToFlash(ushort serialNumber)
        {
            CalibrationStatus = CalibrationResponse.InProgress;
            byte[] msg = new byte[] { (byte)DataPage.Calibration, 0x10, (byte)CTFDefinedId.SerialNumber, 0xFF, 0xFF, 0xFF };
            msg = msg.Concat(BitConverter.GetBytes(serialNumber).Reverse()).ToArray();
            return await SendExtAcknowledgedMessage(msg);
        }
    }
}
