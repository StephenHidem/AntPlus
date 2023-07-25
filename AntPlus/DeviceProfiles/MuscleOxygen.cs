using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmallEarthTech.AntPlus.DeviceProfiles
{
    /// <summary>
    /// This class supports muscle oxygen sensors. This profile is specified in
    /// ANT+ Managed Network Document – Muscle Oxygen Device Profile, Rev 1.1.
    /// </summary>
    /// <seealso cref="AntDevice" />
    public class MuscleOxygen : AntDevice
    {
        private byte eventCount;

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
        public enum MeasurementStatus
        {
            /// <summary>Valid</summary>
            Valid,
            /// <summary>Ambient light too high</summary>
            AmbientLightTooHigh,
            /// <summary>Invalid</summary>
            Invalid
        }

        /// <summary>Total hemoglobin structure.</summary>
        public struct TotalHemoglobin
        {
            /// <summary>Gets or sets the measurement status.</summary>
            public MeasurementStatus Status { get; internal set; }
            /// <summary>Gets or sets the concentration.</summary>
            public double Concentration { get; internal set; }
        }

        /// <summary>Saturated hemoglobin structure.</summary>
        public struct SaturatedHemoglobin
        {
            /// <summary>Gets or sets the measurement status.</summary>
            public MeasurementStatus Status { get; internal set; }
            /// <summary>Gets or sets the percent saturated.</summary>
            public double PercentSaturated { get; internal set; }
        }

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
        public CommonDataPages CommonDataPages { get; private set; }
        /// <inheritdoc/>
        public override Stream DeviceImageStream => typeof(MuscleOxygen).Assembly.GetManifestResourceStream("SmallEarthTech.AntPlus.Images.MuscleOxygen.png");


        /// <summary>Initializes a new instance of the <see cref="MuscleOxygen" /> class.</summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        /// <param name="logger">Logger to use.</param>
        /// <param name="timeout">Time in milliseconds before firing <see cref="AntDevice.DeviceWentOffline"/>.</param>
        public MuscleOxygen(ChannelId channelId, IAntChannel antChannel, ILogger<MuscleOxygen> logger, int timeout = 2000) : base(channelId, antChannel, logger, timeout)
        {
            CommonDataPages = new CommonDataPages(logger);
        }

        /// <inheritdoc/>
        public override void Parse(byte[] dataPage)
        {
            int val;
            TotalHemoglobin thg = new TotalHemoglobin();
            SaturatedHemoglobin shg = new SaturatedHemoglobin();

            base.Parse(dataPage);

            switch ((DataPage)dataPage[0])
            {
                case DataPage.MuscleOxygenData:
                    // return if the event count has not changed
                    if (eventCount == dataPage[1])
                    {
                        return;
                    }
                    eventCount = dataPage[1];
                    UtcTimeRequired = (dataPage[2] & 0x01) == 0x01;
                    SupportsAntFs = (dataPage[3] & 0x01) == 0x01;
                    Interval = (MeasurementInterval)((dataPage[3] >> 1) & 0x07);

                    val = BitConverter.ToUInt16(dataPage, 4) & 0x0FFF;
                    switch (val)
                    {
                        case 0xFFE:
                            thg.Status = MeasurementStatus.AmbientLightTooHigh;
                            thg.Concentration = double.NaN;
                            break;
                        case 0xFFF:
                            thg.Status = MeasurementStatus.Invalid;
                            thg.Concentration = double.NaN;
                            break;
                        default:
                            thg.Status = MeasurementStatus.Valid;
                            thg.Concentration = val * 0.01;
                            break;
                    }
                    TotalHemoglobinConcentration = thg;

                    val = (BitConverter.ToUInt16(dataPage, 5) >> 4) & 0x3FF;
                    switch (val)
                    {
                        case 0x3FE:
                            shg.Status = MeasurementStatus.AmbientLightTooHigh;
                            shg.PercentSaturated = double.NaN;
                            break;
                        case 0x3FF:
                            shg.Status = MeasurementStatus.Invalid;
                            shg.PercentSaturated = double.NaN;
                            break;
                        default:
                            shg.Status = MeasurementStatus.Valid;
                            shg.PercentSaturated = val * 0.1;
                            break;
                    }
                    PreviousSaturatedHemoglobin = shg;

                    val = (BitConverter.ToUInt16(dataPage, 6) >> 6) & 0x03FF;
                    switch (val)
                    {
                        case 0x3FE:
                            shg.Status = MeasurementStatus.AmbientLightTooHigh;
                            shg.PercentSaturated = double.NaN;
                            break;
                        case 0x3FF:
                            shg.Status = MeasurementStatus.Invalid;
                            shg.PercentSaturated = double.NaN;
                            break;
                        default:
                            shg.Status = MeasurementStatus.Valid;
                            shg.PercentSaturated = val * 0.1;
                            break;
                    }
                    CurrentSaturatedHemoglobin = shg;

                    RaisePropertyChange(nameof(UtcTimeRequired));
                    RaisePropertyChange(nameof(SupportsAntFs));
                    RaisePropertyChange(nameof(Interval));
                    RaisePropertyChange(nameof(TotalHemoglobinConcentration));
                    RaisePropertyChange(nameof(PreviousSaturatedHemoglobin));
                    RaisePropertyChange(nameof(CurrentSaturatedHemoglobin));
                    break;
                default:
                    CommonDataPages.ParseCommonDataPage(dataPage);
                    break;
            }
        }

        /// <summary>Sends a command to the muscle oxygen sensor.</summary>
        /// <param name="command">The command.</param>
        /// <param name="localTimeOffset">The local time offset.</param>
        /// <param name="currentTimeStamp">The current time stamp.</param>
        /// <returns>The MessagingReturnCode.</returns>
        public async Task<MessagingReturnCode> SendCommand(CommandId command, TimeSpan localTimeOffset, DateTime currentTimeStamp)
        {
            sbyte offset = (sbyte)(localTimeOffset.TotalMinutes / 15);
            uint current = (uint)(currentTimeStamp - new DateTime(1989, 12, 31)).TotalSeconds;
            byte[] msg = { (byte)DataPage.Commands, (byte)command, 0xFF, (byte)offset };
            msg = msg.Concat(BitConverter.GetBytes(current)).ToArray();
            return await SendExtAcknowledgedMessage(msg);
        }
    }
}
