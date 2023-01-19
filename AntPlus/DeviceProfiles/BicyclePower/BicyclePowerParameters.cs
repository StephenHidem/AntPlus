using System;
using System.ComponentModel;

namespace AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// Parameters sub-pages.
    /// </summary>
    public enum Subpage
    {
        /// <summary>The crank parameters</summary>
        CrankParameters = 0x01,
        /// <summary>The power phase configuration</summary>
        PowerPhaseConfiguration = 0x02,
        /// <summary>The rider position configuration</summary>
        RiderPositionConfiguration = 0x04,
        /// <summary>The advanced capabilities 1</summary>
        AdvancedCapabilities1 = 0xFD,
        /// <summary>The advanced capabilities 2</summary>
        AdvancedCapabilities2 = 0xFE
    }

    /// <summary>
    /// Bicycle power parameters.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class Parameters : INotifyPropertyChanged
    {
        private readonly BicyclePower bp;

        /// <summary>
        /// Various crank parameters.
        /// </summary>
        public readonly struct CrankParameters
        {
            /// <summary>
            /// Status of the sensor crank length.
            /// </summary>
            public enum CrankLengthStatus
            {
                /// <summary>Invalid</summary>
                Invalid = 0,
                /// <summary>Default</summary>
                Default = 1,
                /// <summary>Manually set</summary>
                ManuallySet = 2,
                /// <summary>Automatic or fixed</summary>
                AutoOrFixed = 3
            }

            /// <summary>
            /// Crank sensor firmware status.
            /// </summary>
            public enum SensorMisMatchStatus
            {
                /// <summary>Undefined</summary>
                Undefined = 0,
                /// <summary>Right sensor older</summary>
                RightSensorOlder = 0x04,
                /// <summary>Left sensor older</summary>
                LeftSensorOlder = 0x08,
                /// <summary>Identical</summary>
                Identical = 0x0C
            }

            /// <summary>
            /// Crank sensors available.
            /// </summary>
            public enum SensorAvailabilityStatus
            {
                /// <summary>Undefined</summary>
                Undefined = 0,
                /// <summary>Left present</summary>
                LeftPresent = 0x10,
                /// <summary>Right present</summary>
                RightPresent = 0x20,
                /// <summary>Both present</summary>
                BothPresent = 0x30
            }

            /// <summary>
            /// Custom calibration status.
            /// </summary>
            public enum CustomCalibrationStatus
            {
                /// <summary>Not supported</summary>
                NotSupported = 0,
                /// <summary>Not required</summary>
                NotRequired = 0x40,
                /// <summary>Required</summary>
                Required = 0x80
            }

            /// <summary>The crank length in millimeters.</summary>
            public double CrankLength { get; }
            /// <summary>Gets the crank status.</summary>
            public CrankLengthStatus CrankStatus { get; }
            /// <summary>Gets the crank sensor firmware mismatch status.</summary>
            public SensorMisMatchStatus MismatchStatus { get; }
            /// <summary>Gets the right/left crank sensor availability status.</summary>
            public SensorAvailabilityStatus AvailabilityStatus { get; }
            /// <summary>Gets the custom calibration status.</summary>
            public CustomCalibrationStatus CustomCalibration { get; }
            /// <summary>Gets a value indicating whether the sensor is capable of automatically<br />determining crank length.</summary>
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
        /// <summary>
        /// Advanced capabilities.
        /// </summary>
        public readonly struct AdvCapabilities1
        {
            /// <summary>
            /// Interop properties.
            /// </summary>
            [Flags]
            public enum InteropProp
            {
                /// <summary>Default crank length</summary>
                DefaultCrankLength = 1,
                /// <summary>Requires crank length</summary>
                RequiresCrankLength = 2,
            }

            /// <summary>
            /// Interop capabilities.
            /// </summary>
            [Flags]
            public enum InteroperableCapabilies
            {
                /// <summary>None</summary>
                None = 0,
                /// <summary>4 Hz</summary>
                FourHz = 1,
                /// <summary>8 Hz</summary>
                EightHz = 2,
                /// <summary>Auto zero</summary>
                AutoZero = 16,
                /// <summary>Auto crank length</summary>
                AutoCrankLength = 32,
                /// <summary>Torque effectiveness and pedal smoothness</summary>
                TEandPS = 64,
            }

            /// <summary>Gets the interoperable properties.</summary>
            public InteropProp InteroperableProperties { get; }
            /// <summary>Gets the custom properties.</summary>
            public byte CustomProperties { get; }
            /// <summary>Gets the interoperable capabilities mask.</summary>
            public InteroperableCapabilies Mask { get; }
            /// <summary>Gets the custom capabilities mask.</summary>
            public byte CustomCapabilitiesMask { get; }
            /// <summary>Gets the interoperable capabilities value.</summary>
            public InteroperableCapabilies Value { get; }
            /// <summary>Gets the custom capabilities value.</summary>
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
        /// <summary>
        /// Advanced capabilities.
        /// </summary>
        public readonly struct AdvCapabilities2
        {
            /// <summary>Gets the interoperable capabilities.</summary>
            [Flags]
            public enum InteroperableCapabilies
            {
                /// <summary>None</summary>
                None = 0,
                /// <summary>Four Hz</summary>
                FourHz = 1,
                /// <summary>Eight Hz</summary>
                EightHz = 2,
                /// <summary>Power phase, eight Hz</summary>
                PowerPhase8Hz = 8,
                /// <summary>PCO, eight Hz</summary>
                PCO8Hz = 16,
                /// <summary>Rider position, eight Hz</summary>
                RiderPosition8Hz = 32,
                /// <summary>Troque barycenter, eight Hz</summary>
                TorqueBarycenter8Hz = 64,
            }

            /// <summary>Gets the interoperable capabilities mask.</summary>
            public InteroperableCapabilies Mask { get; }
            /// <summary>Gets the interoperable capabilities value.</summary>
            public InteroperableCapabilies Value { get; }

            internal AdvCapabilities2(byte[] dataPage)
            {
                Mask = (InteroperableCapabilies)dataPage[4];
                Value = (InteroperableCapabilies)dataPage[6];
            }
        }

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Gets the crank parameters.</summary>
        public CrankParameters Crank { get; private set; }
        /// <summary>Gets the peak torque threshold percentage.</summary>
        public double PeakTorqueThreshold { get; private set; }
        /// <summary>Gets the rider position time offset.</summary>
        public byte RiderPositionTimeOffset { get; private set; }
        /// <summary>Gets the advanced capabilities from the advanced setting subpage.</summary>
        public AdvCapabilities1 AdvancedCapabilities1 { get; private set; }
        /// <summary>Gets the advanced capabilities from the advanced setting subpage.</summary>
        public AdvCapabilities2 AdvancedCapabilities2 { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameters"/> class.
        /// </summary>
        /// <param name="bp">The bp.</param>
        public Parameters(BicyclePower bp)
        {
            this.bp = bp;
        }

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public void Parse(byte[] dataPage)
        {
            switch ((Subpage)dataPage[1])
            {
                case Subpage.CrankParameters:
                    Crank = new CrankParameters(dataPage);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Crank)));
                    break;
                case Subpage.PowerPhaseConfiguration:
                    PeakTorqueThreshold = dataPage[2] * 0.5;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PeakTorqueThreshold)));
                    break;
                case Subpage.RiderPositionConfiguration:
                    RiderPositionTimeOffset = dataPage[2];
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RiderPositionTimeOffset)));
                    break;
                case Subpage.AdvancedCapabilities1:
                    AdvancedCapabilities1 = new AdvCapabilities1(dataPage);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AdvancedCapabilities1)));
                    break;
                case Subpage.AdvancedCapabilities2:
                    AdvancedCapabilities2 = new AdvCapabilities2(dataPage);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AdvancedCapabilities2)));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Gets the requested parameters subpage.
        /// </summary>
        /// <param name="parameterSubpage">The parameter subpage.</param>
        public void GetParameters(Subpage parameterSubpage)
        {
            bp.RequestDataPage(DataPage.GetSetParameters, (byte)parameterSubpage);
        }

        /// <summary>
        /// Sets the length of the crank or enables auto crank length. If the crank length is greater than
        /// 236.5 mm, auto crank length is enabled. Typically you would omit the length parameter to enable auto crank length.
        /// </summary>
        /// <param name="length">The length in millimeters. Omit to enable auto crank length.</param>
        public void SetCrankLength(double length = 237)
        {
            byte[] msg;
            if (length > 236.5)
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

        /// <summary>
        /// Sets the rider position transition time offset.
        /// </summary>
        /// <remarks>
        /// A transition state is a state in which the power meter has detected a change from standing to sitting or sitting to standing
        /// but is uncertain whether that change is a true transition or if the transition detected is a false positive.The transition time
        /// offset specifies the time-period between when a transition is first detected until when a determination can be made as to
        /// whether the transition was successful or not.
        /// </remarks>
        /// <param name="offset">The offset in seconds.</param>
        public void SetTransitionTimeOffset(byte offset)
        {
            byte[] msg = new byte[] { (byte)DataPage.GetSetParameters, (byte)Subpage.RiderPositionConfiguration, offset, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            bp.SendExtAcknowledgedMessage(msg);
        }

        /// <summary>
        /// Sets the peak torque threshold percentage.
        /// </summary>
        /// <remarks>
        /// The percentage of the total applied torque during a time interval which is considered to be peak. Setting this value to a low
        /// percentage such as 1% will yield start and end peak torque angles in the left and right force angle pages that more closely
        /// approximate torque barycenter.At 100%, Peak Torque equals Total Torque.
        /// </remarks>
        /// <param name="threshold">The threshold percentage.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Parameter threshold range is 0 to 100 percent.</exception>
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
