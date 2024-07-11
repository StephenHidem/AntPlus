using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// The standard bicycle power crank torque sensor class.
    /// </summary>
    /// <seealso cref="TorqueSensor" />
    public partial class StandardCrankTorqueSensor : TorqueSensor
    {
        /// <summary>Force angle structure.</summary>
        public readonly struct ForceAngle
        {
            /// <summary>Gets the event count.</summary>
            public byte EventCount { get; }
            /// <summary>Gets the start angle.</summary>
            /// <value>The start angle in decimal degrees. NaN if invalid.</value>
            public double StartAngle { get; }
            /// <summary>Gets the end angle.</summary>
            /// <value>The end angle in decimal degrees. NaN if invalid.</value>
            public double EndAngle { get; }
            /// <summary>Gets the start peak angle.</summary>
            /// <value>The start peak angle in decimal degrees. NaN if invalid.</value>
            public double StartPeakAngle { get; }
            /// <summary>Gets the end peak angle.</summary>
            /// <value>The end peak angle in decimal degrees. NaN if invalid.</value>
            public double EndPeakAngle { get; }
            /// <summary>Gets the average torque.</summary>
            /// <value>The average torque in Nm.</value>
            public double AvgTorque { get; }

            internal ForceAngle(byte[] dataPage)
            {
                EventCount = dataPage[1];
                if (dataPage[2] == 0xC0 && dataPage[3] == 0xC0)
                {
                    StartAngle = EndAngle = double.NaN;
                }
                else
                {
                    StartAngle = 360.0 * dataPage[2] / 256.0;
                    EndAngle = 360.0 * dataPage[3] / 256.0;
                }

                if (dataPage[4] == 0xC0 && dataPage[5] == 0xC0)
                {
                    StartPeakAngle = EndPeakAngle = double.NaN;
                }
                else
                {
                    StartPeakAngle = 360.0 * dataPage[4] / 256.0;
                    EndPeakAngle = 360.0 * dataPage[5] / 256.0;
                }

                AvgTorque = BitConverter.ToUInt16(dataPage, 6) / 32.0;
            }
        }

        /// <summary>Pedal position structure.</summary>
        public readonly struct PedalPositionPage
        {
            /// <summary>Rider position.</summary>
            public enum Position
            {
                /// <summary>Seated</summary>
                Seated,
                /// <summary>Transition to seated</summary>
                TransitionToSeated,
                /// <summary>Standing</summary>
                Standing,
                /// <summary>Transition to standing</summary>
                TransitionToStanding
            }

            /// <summary>Gets the crank cycle count.</summary>
            public byte CrankCycleCount { get; }
            /// <summary>Gets the rider position.</summary>
            public Position RiderPosition { get; }
            /// <summary>Gets the average cadence.</summary>
            /// <value>The average cadence in rotations per minute.</value>
            public byte AverageCadence { get; }
            /// <summary>Gets the right platform center offset in millimeters.</summary>
            public sbyte RightPlatformCenterOffset { get; }
            /// <summary>Gets the left platform center offset in millimeters.</summary>
            public sbyte LeftPlatformCenterOffset { get; }

            internal PedalPositionPage(byte[] dataPage)
            {
                CrankCycleCount = dataPage[1];
                RiderPosition = (Position)(dataPage[2] >> 6);
                AverageCadence = dataPage[3];
                RightPlatformCenterOffset = (sbyte)dataPage[4];
                LeftPlatformCenterOffset = (sbyte)dataPage[5];
            }
        }

        /// <summary>Gets the average cadence in rotations per minute.</summary>
        [ObservableProperty]
        private double averageCadence;
        /// <summary>Gets the torque barycenter angle in degrees.</summary>
        [ObservableProperty]
        private double torqueBarycenterAngle;
        /// <summary>Gets the right pedal force angle.</summary>
        [ObservableProperty]
        private ForceAngle rightForceAngle;
        /// <summary>Gets the left pedal force angle.</summary>
        [ObservableProperty]
        private ForceAngle leftForceAngle;
        /// <summary>Gets the pedal position data.</summary>
        [ObservableProperty]
        private PedalPositionPage pedalPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardCrankTorqueSensor"/> class.
        /// </summary>
        /// <param name="logger">Logger to use.</param>
        public StandardCrankTorqueSensor(ILogger logger) : base(logger)
        {
        }

        /// <inheritdoc/>
        public override void ParseTorque(byte[] dataPage)
        {
            base.ParseTorque(dataPage);
            if (deltaEventCount != 0)
            {
                AverageCadence = 60.0 * deltaEventCount / (deltaPeriod / 2048.0);
            }
        }

        /// <inheritdoc/>
        public override string ToString() => "Bike Power (Crank Torque)";

        /// <summary>Parses the cycling dynamics data pages.</summary>
        /// <param name="dataPage">The data page.</param>
        public void ParseCyclingDynamics(byte[] dataPage)
        {
            switch ((DataPage)dataPage[0])
            {
                case DataPage.TorqueBarycenter:
                    TorqueBarycenterAngle = dataPage[1] * 0.5 + 30.0;
                    break;
                case DataPage.RightForceAngle:
                    RightForceAngle = new ForceAngle(dataPage);
                    break;
                case DataPage.LeftForceAngle:
                    LeftForceAngle = new ForceAngle(dataPage);
                    break;
                case DataPage.PedalPosition:
                    PedalPosition = new PedalPositionPage(dataPage);
                    break;
                default:
                    _logger.LogWarning("ParseCyclingDynamics: Unknown data page = {Page}", dataPage[0]);
                    break;
            }
        }
    }
}
