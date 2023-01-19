using System;

namespace AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// The standard crank torque sensor class.
    /// </summary>
    /// <seealso cref="AntPlus.DeviceProfiles.BicyclePower.TorqueSensor" />
    public class StandardCrankTorqueSensor : TorqueSensor
    {
        /// <summary>Force angle structure.</summary>
        public readonly struct ForceAngle
        {
            /// <summary>Gets the event count.</summary>
            public byte EventCount { get; }
            /// <summary>Gets the start angle.</summary>
            /// <value>The start angle in brads (binary radians).</value>
            public byte StartAngle { get; }
            /// <summary>Gets the end angle.</summary>
            /// <value>The end angle in brads (binary radians).</value>
            public byte EndAngle { get; }
            /// <summary>Gets the start peak angle.</summary>
            /// <value>The start peak angle in brads (binary radians).</value>
            public byte StartPeakAngle { get; }
            /// <summary>Gets the end peak angle.</summary>
            /// <value>The end peak angle in brads (binary radians).</value>
            public byte EndPeakAngle { get; }
            /// <summary>Gets the average torque.</summary>
            /// <value>The average torque in Nm.</value>
            public double AvgTorque { get; }

            internal ForceAngle(byte[] dataPage)
            {
                EventCount = dataPage[1];
                StartAngle = dataPage[2];
                EndAngle = dataPage[3];
                StartPeakAngle = dataPage[4];
                EndPeakAngle = dataPage[5];
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
        public double AverageCadence { get; private set; }
        /// <summary>Gets the torque barycenter angle in degrees.</summary>
        public double TorqueBarycenterAngle { get; private set; }
        /// <summary>Gets the right pedal force angle.</summary>
        public ForceAngle RightForceAngle { get; private set; }
        /// <summary>Gets the left pedal force angle.</summary>
        public ForceAngle LeftForceAngle { get; private set; }
        /// <summary>Gets the pedal position data.</summary>
        public PedalPositionPage PedalPosition { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardCrankTorqueSensor"/> class.
        /// </summary>
        /// <param name="bp">The bp.</param>
        public StandardCrankTorqueSensor(BicyclePower bp) : base(bp)
        {
        }

        /// <summary>Parses the crank torque.</summary>
        /// <param name="dataPage">The data page.</param>
        public override void ParseTorque(byte[] dataPage)
        {
            base.ParseTorque(dataPage);
            if (deltaEventCount != 0)
            {
                AverageCadence = 60.0 * deltaEventCount / (deltaPeriod / 2048.0);
                RaisePropertyChange(nameof(AverageCadence));
            }
        }

        /// <summary>Parses the cycling dynamics data pages.</summary>
        /// <param name="dataPage">The data page.</param>
        internal void ParseCyclingDynamics(byte[] dataPage)
        {
            switch ((DataPage)dataPage[0])
            {
                case DataPage.TorqueBarycenter:
                    TorqueBarycenterAngle = dataPage[2] * 0.5 + 30.0;
                    RaisePropertyChange(nameof(TorqueBarycenterAngle));
                    break;
                case DataPage.RightForceAngle:
                    RightForceAngle = new ForceAngle(dataPage);
                    RaisePropertyChange(nameof(RightForceAngle));
                    break;
                case DataPage.LeftForceAngle:
                    LeftForceAngle = new ForceAngle(dataPage);
                    RaisePropertyChange(nameof(LeftForceAngle));
                    break;
                case DataPage.PedalPosition:
                    PedalPosition = new PedalPositionPage(dataPage);
                    RaisePropertyChange(nameof(PedalPosition));
                    break;
                default:
                    break;
            }
        }
    }
}
