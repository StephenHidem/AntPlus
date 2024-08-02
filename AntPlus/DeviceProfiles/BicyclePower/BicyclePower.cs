using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System.Threading.Tasks;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>Base class for bicycle power sensors.</summary>
    /// <remarks>
    /// Bicycle Power sensors fall into two main categories - <see cref="StandardPowerSensor"/> and <see cref="CrankTorqueFrequencySensor"/>.
    /// <para>
    /// Standard power sensors may support <see cref="TorqueSensor"/> in addition to the power only profile. The property <see cref="StandardPowerSensor.TorqueSensor"/>
    /// is set when a <see cref="StandardCrankTorqueSensor"/> or <see cref="StandardWheelTorqueSensor"/> message is received.
    /// </para>
    /// <para>
    /// The <see cref="CrankTorqueFrequencySensor"/> class is somewhat unique among bicycle power sensors. It provides limited commonality and functionality compared to
    /// StandardPowerSensors.
    /// </para>
    /// </remarks>
    public abstract partial class BicyclePower : AntDevice
    {
        /// <summary>
        /// The device type value transmitted in the channel ID.
        /// </summary>
        public const byte DeviceClass = 11;

        /// <inheritdoc/>
        public override int ChannelCount => 8182;

        /// <summary>
        /// Main data pages.
        /// </summary>
        public enum DataPage
        {
            /// <summary>Unknown</summary>
            Unknown,
            /// <summary>Calibration</summary>
            Calibration,
            /// <summary>Get/Set Parameters</summary>
            GetSetParameters,
            /// <summary>Measurement Output</summary>
            MeasurementOutput,
            /// <summary>Power Only</summary>
            PowerOnly = 0x10,
            /// <summary>Wheel Torque</summary>
            WheelTorque,
            /// <summary>Crank Torque</summary>
            CrankTorque,
            /// <summary>Torque effectiveness and pedal smoothness</summary>
            TorqueEffectivenessAndPedalSmoothness,
            /// <summary>Torque barycenter</summary>
            TorqueBarycenter,
            /// <summary>Crank torque frequency</summary>
            CrankTorqueFrequency = 0x20,
            /// <summary>Right force angle</summary>
            RightForceAngle = 0xE0,
            /// <summary>Left force angle</summary>
            LeftForceAngle = 0xE1,
            /// <summary>Pedal position</summary>
            PedalPosition = 0xE2
        }

        /// <summary>The calibration operation status common to all bicycle power sensors.</summary>
        [ObservableProperty]
        private CalibrationResponse calibrationStatus;

        /// <summary>Initializes a new instance of the <see cref="BicyclePower" /> class.</summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, int?, byte?)"/>
        public BicyclePower(ChannelId channelId, IAntChannel antChannel, ILogger logger, int? timeout = default, byte? missedMessages = default)
            : base(channelId, antChannel, logger, timeout, missedMessages)
        {
        }

        /// <summary>Requests manual calibration.</summary>
        /// <returns><see cref="MessagingReturnCode"/></returns>
        public virtual async Task<MessagingReturnCode> RequestManualCalibration()
        {
            CalibrationStatus = CalibrationResponse.InProgress;
            return await SendExtAcknowledgedMessage(new byte[] { (byte)DataPage.Calibration, (byte)CalibrationRequestId.ManualZero, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        }

        /// <summary>Gets the bicycle power sensor.</summary>
        /// <param name="dataPage">The data page.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">The ANT channel.</param>
        /// <param name="loggerFactory">The logger factory to use to create loggers for variants of bicycle power.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="missedMessages">The number of missed messages before signaling the device went offline.</param>
        /// <returns>
        /// A <see cref="CrankTorqueFrequencySensor"/> is returned if a <see cref="DataPage.CrankTorqueFrequency"/> page has been received.
        /// Otherwise, a <see cref="StandardPowerSensor"/> is returned.
        /// </returns>
        public static BicyclePower GetBicyclePowerSensor(byte[] dataPage, ChannelId channelId, IAntChannel antChannel, ILoggerFactory loggerFactory, int? timeout, byte? missedMessages = default)
        {
            if ((DataPage)dataPage[0] == DataPage.CrankTorqueFrequency)
            {
                // return CTF sensor
                CrankTorqueFrequencySensor sensor = new CrankTorqueFrequencySensor(channelId, antChannel, loggerFactory.CreateLogger<CrankTorqueFrequencySensor>(), timeout, missedMessages);
                sensor.Parse(dataPage);
                return sensor;
            }
            else
            {
                StandardPowerSensor sensor = new StandardPowerSensor(channelId, antChannel, loggerFactory.CreateLogger<StandardPowerSensor>(), timeout, missedMessages);
                sensor.Parse(dataPage);
                return sensor;
            }
        }
    }
}
