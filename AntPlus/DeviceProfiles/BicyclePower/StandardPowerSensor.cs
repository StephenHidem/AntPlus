using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.IO;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// The standard power sensor class. Note that torque sensors report this data page for
    /// displays that may not handle torque sensor messages.
    /// </summary>
    public partial class StandardPowerSensor : BicyclePower
    {
        /// <inheritdoc/>
        public override Stream DeviceImageStream => typeof(StandardPowerSensor).Assembly.GetManifestResourceStream("SmallEarthTech.AntPlus.Images.BicyclePower.png");
        /// <inheritdoc/>
        public override string ToString() => "Bike Power";

        private bool isFirstDataMessage = true;     // used for accumulated values
        private byte lastEventCount;
        private int deltaEventCount;
        private ushort lastPower;
        private int deltaPower;

        /// <summary>Pedal power differentiation.</summary>
        public enum PedalDifferentiation
        {
            /// <summary>Right pedal power contribution.</summary>
            RightPedal,
            /// <summary>Unknown pedal power contribution.</summary>
            Unknown,
            /// <summary>Pedal power not used.</summary>
            Unused
        }

        /// <summary>Gets the average power in watts.</summary>
        [ObservableProperty]
        private double averagePower;
        /// <summary>The pedal power data field provides the user’s power contribution (as a percentage) between the left and right pedals, as
        /// measured by a pedal power sensor. </summary>
        /// <value>The pedal power. 127 indicates pedal power is unused. Check <see cref="PedalContribution"/>.</value>
        [ObservableProperty]
        private byte pedalPower;
        /// <summary>Gets the pedal power contribution.</summary>
        /// <value>The pedal differentiation.</value>
        [ObservableProperty]
        private PedalDifferentiation pedalContribution;
        /// <summary>Gets the instantaneous pedaling cadence. 0xFF indicates invalid.</summary>
        [ObservableProperty]
        private byte instantaneousCadence;
        /// <summary>Gets the instantaneous power in watts.</summary>
        [ObservableProperty]
        private ushort instantaneousPower;

        /// <summary>Gets the torque sensor.</summary>
        /// <value>The wheel or crank torque sensor.</value>
        [ObservableProperty]
        private TorqueSensor? torqueSensor;

        /// <summary>Gets the common data pages.</summary>
        public CommonDataPages CommonDataPages { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardPowerSensor"/> class.
        /// </summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, int)"/>
        public StandardPowerSensor(ChannelId channelId, IAntChannel antChannel, ILogger<StandardPowerSensor> logger, int timeout)
            : base(channelId, antChannel, logger, timeout)
        {
            CommonDataPages = new CommonDataPages(logger);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardPowerSensor"/> class.
        /// </summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, TimeoutOptions?)"/>
        public StandardPowerSensor(ChannelId channelId, IAntChannel antChannel, ILogger<StandardPowerSensor> logger, TimeoutOptions? options)
            : base(channelId, antChannel, logger, options)
        {
            CommonDataPages = new CommonDataPages(logger);
        }

        /// <inheritdoc/>
        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);
            switch ((DataPage)dataPage[0])
            {
                case DataPage.Calibration:
                    ParseCalibrationPage(dataPage);
                    break;
                case DataPage.GetSetParameters:
                    ParseParameters(dataPage);
                    break;
                case DataPage.MeasurementOutput:
                    ParseMeasurementOutputData(dataPage);
                    break;
                case DataPage.PowerOnly:
                    ParsePowerOnly(dataPage);
                    break;
                case DataPage.WheelTorque:
                    TorqueSensor ??= new StandardWheelTorqueSensor(_logger);
                    TorqueSensor.ParseTorque(dataPage);
                    break;
                case DataPage.CrankTorque:
                    TorqueSensor ??= new StandardCrankTorqueSensor(_logger);
                    TorqueSensor.ParseTorque(dataPage);
                    break;
                case DataPage.TorqueEffectivenessAndPedalSmoothness:
                    ParseTEPS(dataPage);
                    break;
                case DataPage.TorqueBarycenter:
                case DataPage.RightForceAngle:
                case DataPage.LeftForceAngle:
                case DataPage.PedalPosition:
                    if (TorqueSensor is StandardCrankTorqueSensor sensor)
                    {
                        sensor.ParseCyclingDynamics(dataPage);
                    }
                    else
                    {
                        _logger.LogWarning("Unexpected Cycling Dynamics message. A StandardCrankTorqueSensor is not present.");
                    }
                    break;
                default:
                    CommonDataPages.ParseCommonDataPage(dataPage);
                    break;
            }
        }

        private void ParsePowerOnly(byte[] dataPage)
        {
            PedalPower = (byte)(dataPage[2] & 0x7F);
            PedalContribution = dataPage[2] == 0xFF
                ? PedalDifferentiation.Unused
                : (dataPage[2] & 0x80) != 0 ? PedalDifferentiation.RightPedal : PedalDifferentiation.Unknown;
            InstantaneousCadence = dataPage[3];
            InstantaneousPower = BitConverter.ToUInt16(dataPage, 6);

            if (isFirstDataMessage)
            {
                // initialize if first data message
                isFirstDataMessage = false;
                lastEventCount = dataPage[1];
                lastPower = BitConverter.ToUInt16(dataPage, 4);
                return;
            }

            if (dataPage[1] != lastEventCount)
            {
                // handle new events
                deltaEventCount = Utils.CalculateDelta(dataPage[1], ref lastEventCount);
                deltaPower = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 4), ref lastPower);
                AveragePower = deltaPower / deltaEventCount;
            }
        }
    }
}
