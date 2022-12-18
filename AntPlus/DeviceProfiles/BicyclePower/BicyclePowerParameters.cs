using System;

namespace AntPlus.DeviceProfiles.BicyclePower
{
    public enum Subpage
    {
        CrankParameters = 0x01,
        PowerPhaseConfiguration = 0x02,
        RiderPositionConfiguration = 0x04,
        AdvancedCapabilities1 = 0xFD,
        AdvancedCapabilities2 = 0xFE
    }

    public class Parameters
    {
        public readonly struct CrankParameters
        {
            public enum CrankLengthStatus
            {
                Invalid = 0,
                Default = 1,
                ManuallySet = 2,
                AutoOrFixed = 3
            }

            public enum SensorMisMatchStatus
            {
                Undefined = 0,
                RightSensorOlder = 0x04,
                LeftSensorOlder = 0x08,
                Identical = 0x0C
            }

            public enum SensorAvailabilityStatus
            {
                Undefined = 0,
                LeftPresent = 0x10,
                RightPresent = 0x20,
                BothPresent = 0x30
            }

            public enum CustomCalibrationStatus
            {
                NotSupported = 0,
                NotRequired = 0x40,
                Required = 0x80
            }

            /// <summary>The crank length in millimeters.</summary>
            public double CrankLength { get; }
            public CrankLengthStatus CrankStatus { get; }
            public SensorMisMatchStatus MismatchStatus { get; }
            public SensorAvailabilityStatus AvailabilityStatus { get; }
            public CustomCalibrationStatus CustomCalibration { get; }
            public bool AutoCrankLength { get; }

            internal CrankParameters(byte[] dataPage)
            {
                CrankLength = dataPage[4] * 0.5 + 110.0;
                CrankStatus = (CrankLengthStatus)(dataPage[5] & 0x03);
                MismatchStatus = (SensorMisMatchStatus)(dataPage[5] & 0x0C);
                AvailabilityStatus = (SensorAvailabilityStatus)(dataPage[5] & 0x30);
                CustomCalibration = (CustomCalibrationStatus)(dataPage[5] & 0xC0);
                AutoCrankLength = (dataPage[6] & 0x01) != 0;
            }
        }
        public readonly struct AdvCapabilities1
        {
            [Flags]
            public enum InteropProp
            {
                DefaultCrankLength = 1,
                RequiresCrankLength = 2,
            }

            [Flags]
            public enum InteroperableCapabilies
            {
                None = 0,
                FourHz = 1,
                EightHz = 2,
                AutoZero = 16,
                AutoCrankLength = 32,
                TEandPS = 64,
            }

            public InteropProp InteroperableProperties { get; }
            public byte CustomProperties { get; }
            public InteroperableCapabilies Mask { get; }
            public byte CustomCapabilitiesMask { get; }
            public InteroperableCapabilies Value { get; }
            public byte CustomCapabilitiesValue { get; }

            internal AdvCapabilities1(byte[] dataPage)
            {
                InteroperableProperties = (InteropProp)(dataPage[2] & 0x03);
                CustomProperties = dataPage[3];
                Mask = (InteroperableCapabilies)dataPage[4];
                CustomCapabilitiesMask = dataPage[5];
                Value = (InteroperableCapabilies)dataPage[6];
                CustomCapabilitiesValue = dataPage[7];
            }
        }
        public readonly struct AdvCapabilities2
        {
            [Flags]
            public enum InteroperableCapabilies
            {
                None = 0,
                FourHz = 1,
                EightHz = 2,
                PowerPhase8Hz = 8,
                PCO8Hz = 16,
                RiderPosition8Hz = 32,
                TorqueBarycenter8Hz = 64,
            }

            public InteroperableCapabilies Mask { get; }
            public InteroperableCapabilies Value { get; }

            internal AdvCapabilities2(byte[] dataPage)
            {
                Mask = (InteroperableCapabilies)dataPage[4];
                Value = (InteroperableCapabilies)dataPage[6];
            }
        }

        private readonly BicyclePower bp;

        public CrankParameters Crank { get; private set; }
        public double PeakTorqueThreshold { get; private set; }
        public byte RiderPositionTimeOffset { get; private set; }
        public AdvCapabilities1 AdvancedCapabilities1 { get; private set; }
        public AdvCapabilities2 AdvancedCapabilities2 { get; private set; }

        public Parameters(BicyclePower bp)
        {
            this.bp = bp;
        }

        public void Parse(byte[] dataPage)
        {
            switch ((Subpage)dataPage[1])
            {
                case Subpage.CrankParameters:
                    Crank = new CrankParameters(dataPage);
                    break;
                case Subpage.PowerPhaseConfiguration:
                    PeakTorqueThreshold = dataPage[2] * 0.5;
                    break;
                case Subpage.RiderPositionConfiguration:
                    RiderPositionTimeOffset = dataPage[2];
                    break;
                case Subpage.AdvancedCapabilities1:
                    AdvancedCapabilities1 = new AdvCapabilities1(dataPage);
                    break;
                case Subpage.AdvancedCapabilities2:
                    AdvancedCapabilities2 = new AdvCapabilities2(dataPage);
                    break;
                default:
                    break;
            }
        }

        public void GetParameters(Subpage parameterSubpage)
        {
            bp.RequestDataPage(DataPage.GetSetParameters, (byte)parameterSubpage);
        }

        public void SetCrankLength(double length)
        {
            byte[] msg;
            if (length >= 237)
            {
                // set crank length to auto
                msg = new byte[] { (byte)DataPage.GetSetParameters, (byte)Subpage.CrankParameters, 0xFF, 0xFF, 0xFE, 0x00, 0x00, 0xFF };
            }
            else
            {
                byte cl = (byte)((length - 110) / 0.5);
                msg = new byte[] { (byte)DataPage.GetSetParameters, (byte)Subpage.CrankParameters, 0xFF, 0xFF, cl, 0x00, 0x00, 0xFF };
            }
            bp.SendExtAcknowledgedMessage(msg);
        }

        public void SetTransitionTimeOffset(byte offset)
        {
            byte[] msg = new byte[] { (byte)DataPage.GetSetParameters, (byte)Subpage.RiderPositionConfiguration, offset, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            bp.SendExtAcknowledgedMessage(msg);
        }

        public void SetPeakTorqueThreshold(double threshold)
        {
            // valid range is 0 to 100 percent
            if (threshold < 0 || threshold > 100)
            {
                throw new ArgumentOutOfRangeException("Parameter threshold range is 0 to 100 percent.");
            }
            byte peak = (byte)((threshold / 0.5));
            byte[] msg = new byte[] { (byte)DataPage.GetSetParameters, (byte)Subpage.PowerPhaseConfiguration, peak, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            bp.SendExtAcknowledgedMessage(msg);
        }
    }
}
