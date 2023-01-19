using AntRadioInterface;
using System;
using System.Linq;

namespace AntPlus.DeviceProfiles
{
    /// <summary>
    /// This class supports muscle oxygen sensors.
    /// </summary>
    /// <seealso cref="AntPlus.AntDevice" />
    public class MuscleOxygen : AntDevice
    {
        /// <summary>
        /// The muscle oxygen device class ID.
        /// </summary>
        public const byte DeviceClass = 31;

        /// <summary>
        /// Main data pages.
        /// </summary>
        private enum DataPage
        {
            /// <summary>Muscle oxygen data</summary>
            MuscleOxygenData = 0x01,
            /// <summary>Commands</summary>
            Commands = 0x10,
        }

        /// <summary>Command identifiers.</summary>
        public enum CommandId
        {
            /// <summary>Set time.</summary>
            SetTime,
            /// <summary>Start session.</summary>
            StartSession,
            /// <summary>Stop session.</summary>
            StopSession,
            /// <summary>Mark lap.</summary>
            Lap
        }

        /// <summary>The measurement interval used.</summary>
        public enum MeasurementInterval
        {
            /// <summary>None</summary>
            None = 0,
            /// <summary>Quarter second</summary>
            QuarterSecond = 1,
            /// <summary>Half second</summary>
            HalfSecond = 2,
            /// <summary>One second</summary>
            OneSecond = 3,
            /// <summary>Two second</summary>
            TwoSecond = 4
        }

        /// <summary>Measurement status.</summary>
        public enum MeasuremantStatus
        {
            /// <summary>Valid</summary>
            Valid,
            /// <summary>Ambient light too high</summary>
            AmbientLightTooHigh,
            /// <summary>Invalid</summary>
            Invalid
        }

        /// <summary>Total hemoglobin class.</summary>
        public class TotalHemoglobin
        {
            /// <summary>Gets or sets the measurement status.</summary>
            public MeasuremantStatus Status { get; set; }
            /// <summary>Gets or sets the concentration.</summary>
            public double Concentration { get; set; }
        }

        /// <summary>Saturated hemoglobin class.</summary>
        public class SaturatedHemoglobin
        {
            /// <summary>Gets or sets the measurement status.</summary>
            public MeasuremantStatus Status { set; get; }
            /// <summary>Gets or sets the percent saturated.</summary>
            public double PercentSaturated { get; set; }
        }

        /// <summary>Gets the event count.</summary>
        public byte EventCount { get; private set; }
        /// <summary>Gets a value indicating whether UTC time required.</summary>
        public bool UtcTimeRequired { get; private set; }
        /// <summary>Gets a value indicating whether ANT-FS is supported.</summary>
        public bool SupportsAntFs { get; private set; }
        /// <summary>Gets the measurement interval.</summary>
        public MeasurementInterval Interval { get; private set; }
        /// <summary>Gets the total hemoglobin concentration.</summary>
        public TotalHemoglobin TotalHemoglobinConcentration { get; private set; }
        /// <summary>Gets the previous saturated hemoglobin.</summary>
        public SaturatedHemoglobin PreviousSaturatedHemoglobin { get; private set; }
        /// <summary>Gets the current saturated hemoglobin.</summary>
        public SaturatedHemoglobin CurrentSaturatedHemoglobin { get; private set; }
        /// <summary>Gets the common data pages.</summary>
        public CommonDataPages CommonDataPages { get; private set; } = new CommonDataPages();


        /// <summary>Initializes a new instance of the <see cref="MuscleOxygen" /> class.</summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        public MuscleOxygen(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
            TotalHemoglobinConcentration = new TotalHemoglobin();
            PreviousSaturatedHemoglobin = new SaturatedHemoglobin();
            CurrentSaturatedHemoglobin = new SaturatedHemoglobin();
        }

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage"></param>
        public override void Parse(byte[] dataPage)
        {
            int val;

            switch ((DataPage)dataPage[0])
            {
                case DataPage.MuscleOxygenData:
                    EventCount = dataPage[1];
                    UtcTimeRequired = (dataPage[2] & 0x01) == 0x01;
                    SupportsAntFs = (dataPage[3] & 0x01) == 0x01;
                    Interval = (MeasurementInterval)((dataPage[3] >> 1) & 0x07);

                    val = BitConverter.ToUInt16(dataPage, 4) & 0x0FFF;
                    switch (val)
                    {
                        case 0xFFE:
                            TotalHemoglobinConcentration.Status = MeasuremantStatus.AmbientLightTooHigh;
                            break;
                        case 0xFFF:
                            TotalHemoglobinConcentration.Status = MeasuremantStatus.Invalid;
                            break;
                        default:
                            TotalHemoglobinConcentration.Status = MeasuremantStatus.Valid;
                            TotalHemoglobinConcentration.Concentration = val * 0.01;
                            break;
                    }

                    val = (BitConverter.ToUInt16(dataPage, 5) >> 4) & 0x3FF;
                    switch (val)
                    {
                        case 0xFFE:
                            PreviousSaturatedHemoglobin.Status = MeasuremantStatus.AmbientLightTooHigh;
                            break;
                        case 0xFFF:
                            PreviousSaturatedHemoglobin.Status = MeasuremantStatus.Invalid;
                            break;
                        default:
                            PreviousSaturatedHemoglobin.Status = MeasuremantStatus.Valid;
                            PreviousSaturatedHemoglobin.PercentSaturated = val * 0.1;
                            break;
                    }

                    val = (BitConverter.ToUInt16(dataPage, 6) >> 6) & 0x03FF;
                    switch (val)
                    {
                        case 0xFFE:
                            CurrentSaturatedHemoglobin.Status = MeasuremantStatus.AmbientLightTooHigh;
                            break;
                        case 0xFFF:
                            CurrentSaturatedHemoglobin.Status = MeasuremantStatus.Invalid;
                            break;
                        default:
                            CurrentSaturatedHemoglobin.Status = MeasuremantStatus.Valid;
                            CurrentSaturatedHemoglobin.PercentSaturated = val * 0.1;
                            break;
                    }

                    RaisePropertyChange(string.Empty);
                    break;
                default:
                    CommonDataPages.ParseCommonDataPage(dataPage);
                    break;
            }
        }

        /// <summary>Sends the command to the muscle oxygen sensor.</summary>
        /// <param name="command">The command.</param>
        /// <param name="localTimeOffest">The local time offest.</param>
        /// <param name="currentTimeStamp">The current time stamp.</param>
        public void SendCommand(CommandId command, TimeSpan localTimeOffest, DateTime currentTimeStamp)
        {
            sbyte offset = (sbyte)(localTimeOffest.TotalMinutes / 15);
            int current = (int)(currentTimeStamp - new DateTime(1989, 12, 31)).TotalSeconds;
            byte[] msg = { (byte)DataPage.Commands, 0xFF, (byte)command, (byte)offset };
            msg = msg.Concat(BitConverter.GetBytes(current)).ToArray();
            SendExtAcknowledgedMessage(msg);
        }
    }
}
