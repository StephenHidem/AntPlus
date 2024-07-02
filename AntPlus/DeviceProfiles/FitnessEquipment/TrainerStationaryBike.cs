using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Threading.Tasks;

namespace SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment
{
    /// <summary>
    /// This class supports the stationary bike fitness equipment type.
    /// </summary>
    public class TrainerStationaryBike : Equipment
    {
        private bool isFirstDataMessage = true;
        private byte lastEventCount;
        private ushort lastPower;
        private CalibrationRequestResponse _request;

        /// <summary>Initializes a new instance of the <see cref="TrainerStationaryBike" /> class.</summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        /// <param name="logger">Logger to use.</param>
        /// <param name="timeout">Time in milliseconds before firing <see cref="AntDevice.DeviceWentOffline" />.</param>
        public TrainerStationaryBike(ChannelId channelId, IAntChannel antChannel, ILogger<Equipment> logger, int timeout = 2000) : base(channelId, antChannel, logger, timeout)
        {
        }

        //private Equipment _equipment;

        /// <summary>The trainer status bit field is used to indicate whether the trainer requires calibration and/or configuration data to be sent.</summary>
        [Flags]
        public enum TrainerStatusField
        {
            /// <summary>Complete or not required.</summary>
            None = 0x00,
            /// <summary>Bike power calibration required (i.e. zero offset).</summary>
            BikePowerCalRequired = 0x10,
            /// <summary>Resistance calibration required (i.e. spin down).</summary>
            ResistanceCalRequired = 0x20,
            /// <summary>User configuration required.</summary>
            UserConfigRequired = 0x40,
        }

        /// <summary>Used by trainers operating in target power mode.</summary>
        public enum TargetPowerLimit
        {
            /// <summary>At target power or no target power set.</summary>
            AtTargetPower = 0,
            /// <summary>Cycling speed is too low to achieve target power.</summary>
            TooLow = 1,
            /// <summary>Cycling speed is too high to achieve target power.</summary>
            TooHigh = 2,
            /// <summary>Target power limit is undetermined.</summary>
            Undetermined = 3,
        }

        /// <summary> Calibration request/response flags. </summary>
        [Flags]
        public enum CalibrationRequestResponse
        {
            /// <summary>None</summary>
            None = 0,
            /// <summary>Zero offset</summary>
            ZeroOffset = 0x40,
            /// <summary>Spin down</summary>
            SpinDown = 0x80
        }

        /// <summary>Result of the calibration request.</summary>
        [Flags]
        public enum CalibrationResult
        {
            /// <summary>Calibration request succeeded</summary>
            Success = 0x01,
            /// <summary>The temperature is valid</summary>
            TemperatureValid = 0x02,
            /// <summary>The zero offset is valid</summary>
            ZeroOffsetValid = 0x04,
            /// <summary>The spin down time is valid</summary>
            SpinDownValid = 0x08
        }

        /// <summary>Current reported temperature condition.</summary>
        public enum TemperatureCondition
        {
            /// <summary>Not applicable</summary>
            NotApplicable = 0x00,
            /// <summary>Current speed is too low</summary>
            Low = 0x10,
            /// <summary>Current speed is OK</summary>
            Ok = 0x20,
            /// <summary>Current temperature is too high</summary>
            High = 0x30
        }

        /// <summary>Current reported speed condition.</summary>
        public enum SpeedCondition
        {
            /// <summary>Not applicable</summary>
            NotApplicable = 0x00,
            /// <summary>Current temperature is too low</summary>
            Low = 0x40,
            /// <summary>Current temperature is OK</summary>
            Ok = 0x80,
            /// <summary>Reserved</summary>
            Reserved = 0xC0
        }

        /// <summary>Gets the average power in watts.</summary>
        /// <value>The average power.</value>
        public double AveragePower { get; private set; }
        /// <summary>Gets the instantaneous pedaling cadence in revolutions per minute.</summary>
        /// <value>The instantaneous cadence.</value>
        public byte InstantaneousCadence { get; private set; }
        /// <summary>Gets the instantaneous power in watts.</summary>
        /// <value>The instantaneous power.</value>
        public int InstantaneousPower { get; private set; }
        /// <summary>Gets the trainer status.</summary>
        public TrainerStatusField TrainerStatus { get; private set; }
        /// <summary>Gets the trainer target power.</summary>
        public TargetPowerLimit TargetPower { get; private set; }
        /// <summary>Gets the trainer torque.</summary>
        public TrainerTorque TrainerTorque { get; private set; } = new TrainerTorque();

        /// <summary>Gets the result of the requested calibration.</summary>
        /// <value>The rhe result of calibration</value>
        public CalibrationResult CalibrationStatus { get; private set; }
        /// <summary>Gets the current temperature in degrees Celsius.</summary>
        /// <value>The final temperature in degrees C.</value>
        public double Temperature { get; private set; }
        /// <summary>Gets the zero offset reported by the trainer.</summary>
        /// <value>The zero offset.</value>
        public ushort ZeroOffset { get; private set; }
        /// <summary>Gets the spin down time after calibration has been performed.</summary>
        /// <value>The spin down time in milliseconds.</value>
        public ushort SpinDownTime { get; private set; }

        /// <summary>Gets the calibration temperature status.</summary>
        /// <value>The calibration temperature status.</value>
        public TemperatureCondition TemperatureStatus { get; private set; }
        /// <summary>Gets the calibration speed status.</summary>
        /// <value>The calibration speed status.</value>
        public SpeedCondition SpeedStatus { get; private set; }
        /// <summary>Gets the current temperature in degrees Celsius.</summary>
        /// <value>The current temperature in degrees C.</value>
        public double CurrentTemperature { get; private set; }
        /// <summary>Gets the target speed during calibration.</summary>
        /// <value>The target speed in meters per second.</value>
        public double TargetSpeed { get; private set; }
        /// <summary>Gets the target spin down time in milliseconds during calibration.</summary>
        /// <value>The target spin down time in milliseconds.</value>
        public ushort TargetSpinDownTime { get; private set; }

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public override void Parse(byte[] dataPage)
        {
            switch ((DataPage)dataPage[0])
            {
                case DataPage.CalRequestResponse:
                    CalibrationStatus = (dataPage[1] == (byte)_request) ? CalibrationResult.Success : 0;

                    if (dataPage[3] != 0xFF)
                    {
                        CalibrationStatus |= CalibrationResult.TemperatureValid;
                        Temperature = Math.Round(dataPage[3] * 0.5 - 25, 1);
                        RaisePropertyChange(nameof(Temperature));
                    }

                    if (BitConverter.ToUInt16(dataPage, 4) != 0xFFFF)
                    {
                        CalibrationStatus |= CalibrationResult.ZeroOffsetValid;
                        ZeroOffset = BitConverter.ToUInt16(dataPage, 4);
                        RaisePropertyChange(nameof(ZeroOffset));
                    }

                    if (BitConverter.ToUInt16(dataPage, 6) != 0xFFFF)
                    {
                        CalibrationStatus |= CalibrationResult.SpinDownValid;
                        SpinDownTime = BitConverter.ToUInt16(dataPage, 6);
                        RaisePropertyChange(nameof(SpinDownTime));
                    }

                    RaisePropertyChange(nameof(CalibrationStatus));
                    break;
                case DataPage.CalProgress:
                    TemperatureStatus = (TemperatureCondition)(dataPage[2] & 0x30);
                    SpeedStatus = (SpeedCondition)(dataPage[2] & 0xC0);
                    RaisePropertyChange(nameof(TemperatureStatus));
                    RaisePropertyChange(nameof(SpeedStatus));

                    if (dataPage[3] != 0xFF)
                    {
                        CurrentTemperature = Math.Round(dataPage[3] * 0.5 - 25, 1);
                        RaisePropertyChange(nameof(CurrentTemperature));
                    }

                    if (BitConverter.ToUInt16(dataPage, 4) != 0xFFFF)
                    {
                        TargetSpeed = Math.Round(BitConverter.ToUInt16(dataPage, 4) * 0.001, 3);
                        RaisePropertyChange(nameof(TargetSpeed));
                    }

                    if (BitConverter.ToUInt16(dataPage, 6) != 0xFFFF)
                    {
                        TargetSpinDownTime = BitConverter.ToUInt16(dataPage, 6);
                        RaisePropertyChange(nameof(TargetSpinDownTime));
                    }
                    break;
                case DataPage.TrainerStationaryBikeData:
                    HandleFEState(dataPage[7]);
                    InstantaneousCadence = dataPage[2];
                    InstantaneousPower = BitConverter.ToUInt16(dataPage, 5) & 0x0FFF;
                    TrainerStatus = (TrainerStatusField)(dataPage[6] & 0x70);
                    TargetPower = (TargetPowerLimit)(dataPage[7] & 0x0F);
                    RaisePropertyChange(nameof(InstantaneousCadence));
                    RaisePropertyChange(nameof(InstantaneousPower));
                    RaisePropertyChange(nameof(TrainerStatus));
                    RaisePropertyChange(nameof(TargetPower));

                    if (isFirstDataMessage)
                    {
                        // initialize if first data message
                        isFirstDataMessage = false;
                        lastEventCount = dataPage[1];
                        lastPower = BitConverter.ToUInt16(dataPage, 3);
                        return;
                    }

                    if (dataPage[1] != lastEventCount)
                    {
                        // handle new events
                        int deltaEventCount = Utils.CalculateDelta(dataPage[1], ref lastEventCount);
                        int deltaPower = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 3), ref lastPower);
                        AveragePower = deltaPower / deltaEventCount;
                        RaisePropertyChange(nameof(AveragePower));
                    }
                    break;
                case DataPage.TrainerTorqueData:
                    HandleFEState(dataPage[7]);
                    TrainerTorque.Parse(dataPage);
                    RaisePropertyChange(nameof(TrainerTorque));
                    break;
                default:
                    base.Parse(dataPage);
                    break;
            }
        }

        /// <inheritdoc />
        public override string ToString() => "Trainer/Stationary Bike";

        /// <summary>Calibrations the request.</summary>
        /// <param name="request">The request.</param>
        /// <returns><see cref="MessagingReturnCode"/></returns>
        /// <exception cref="System.ArgumentException">Invalid calibration request.</exception>
        public async Task<MessagingReturnCode> CalibrationRequest(CalibrationRequestResponse request)
        {
            if (request == 0)
            {
                throw new ArgumentException("Invalid calibration request.");
            }
            _request = request;
            return await SendExtAcknowledgedMessage(new byte[] { 0x01, (byte)request, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        }
    }
}
