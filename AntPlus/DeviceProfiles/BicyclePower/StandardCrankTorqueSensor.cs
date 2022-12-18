using System;

namespace DeviceProfiles.BicyclePower
{
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

        public readonly struct PedalPosition
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

            public PedalPosition(byte[] dataPage)
            {
                CrankCycleCount = dataPage[1];
                RiderPosition = (Position)(dataPage[2] >> 6);
                AverageCadence = dataPage[3];
                RightPlatformCenterOffset = (sbyte)dataPage[4];
                LeftPlatformCenterOffset = (sbyte)dataPage[5];
            }
        }

        public event EventHandler CrankTorquePageChanged;
        public event EventHandler<double> TorqueBarycenterAngleChanged;
        public event EventHandler<ForceAngle> RightForceAngleChanged;
        public event EventHandler<ForceAngle> LeftForceAngleChanged;
        public event EventHandler<PedalPosition> PedalPositionChanged;

        /// <summary>Gets the average cadence.</summary>
        /// <value>The average cadence in rotations per minute.</value>
        public double AverageCadence { get; private set; }

        public StandardCrankTorqueSensor(BicyclePower bp) : base(bp)
        {
        }

        public override void ParseTorque(byte[] dataPage)
        {
            bool firstPage = isFirstDataMessage; // save first message flag for later use
            base.ParseTorque(dataPage);
            if (!firstPage)
            {
                AverageCadence = 60.0 * deltaEventCount / (deltaPeriod / 2048.0);
            }
            CrankTorquePageChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ParseCyclingDynamics(byte[] dataPage)
        {
            switch ((DataPage)dataPage[0])
            {
                case DataPage.TorqueBarycenter:
                    TorqueBarycenterAngleChanged?.Invoke(this, dataPage[2] * 0.5 + 30.0);
                    break;
                case DataPage.RightForceAngle:
                    RightForceAngleChanged?.Invoke(this, new ForceAngle(dataPage));
                    break;
                case DataPage.LeftForceAngle:
                    LeftForceAngleChanged?.Invoke(this, new ForceAngle(dataPage));
                    break;
                case DataPage.PedalPosition:
                    PedalPositionChanged?.Invoke(this, new PedalPosition(dataPage));
                    break;
                default:
                    break;
            }
        }
    }
}
