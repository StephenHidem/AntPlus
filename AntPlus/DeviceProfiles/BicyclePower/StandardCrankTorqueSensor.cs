using System;

namespace AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// The standard crank torque sensor class.
    /// </summary>
    /// <seealso cref="AntPlus.DeviceProfiles.BicyclePower.TorqueSensor" />
    public class StandardCrankTorqueSensor : TorqueSensor
    {
        public readonly struct ForceAngle
        {
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

            public ForceAngle(byte[] dataPage)
            {
                EventCount = dataPage[1];
                StartAngle = dataPage[2];
                EndAngle = dataPage[3];
                StartPeakAngle = dataPage[4];
                EndPeakAngle = dataPage[5];
                AvgTorque = BitConverter.ToUInt16(dataPage, 6) / 32.0;
            }
        }

        public readonly struct PedalPositionPage
        {
            public enum Position
            {
                Seated,
                TransitionToSeated,
                Standing,
                TransitionToStanding
            }

            public byte CrankCycleCount { get; }
            public Position RiderPosition { get; }
            /// <summary>Gets the average cadence.</summary>
            /// <value>The average cadence in rotations per minute.</value>
            public byte AverageCadence { get; }
            public sbyte RightPlatformCenterOffset { get; }
            public sbyte LeftPlatformCenterOffset { get; }

            public PedalPositionPage(byte[] dataPage)
            {
                CrankCycleCount = dataPage[1];
                RiderPosition = (Position)(dataPage[2] >> 6);
                AverageCadence = dataPage[3];
                RightPlatformCenterOffset = (sbyte)dataPage[4];
                LeftPlatformCenterOffset = (sbyte)dataPage[5];
            }
        }

        /// <summary>Gets the average cadence.</summary>
        /// <value>The average cadence in rotations per minute.</value>
        public double AverageCadence { get; private set; }
        public double TorqueBarycenterAngle { get; private set; }
        public ForceAngle RightForceAngle { get; private set; }
        public ForceAngle LeftForceAngle { get; private set; }
        public PedalPositionPage PedalPosition { get; private set; }

        public StandardCrankTorqueSensor(BicyclePower bp) : base(bp)
        {
        }

        public override void ParseTorque(byte[] dataPage)
        {
            base.ParseTorque(dataPage);
            if (deltaEventCount != 0)
            {
                AverageCadence = 60.0 * deltaEventCount / (deltaPeriod / 2048.0);
                RaisePropertyChange(nameof(AverageCadence));
            }
        }

        public void ParseCyclingDynamics(byte[] dataPage)
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
