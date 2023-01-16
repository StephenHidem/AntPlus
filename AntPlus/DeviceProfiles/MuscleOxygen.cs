using AntRadioInterface;
using System;
using System.Linq;

namespace AntPlus.DeviceProfiles
{
    public class MuscleOxygen : AntDevice
    {
        /// The fitness equipment device class ID.
        /// </summary>
        public const byte DeviceClass = 31;

        /// <summary>
        /// Main data pages.
        /// </summary>
        private enum DataPage
        {
            MuscleOxygenData = 0x01,
            Commands = 0x10,
        }

        public enum CommandId
        {
            SetTime,
            StartSession,
            StopSession,
            Lap
        }

        public enum MeasurementInterval
        {
            None = 0,
            QuarterSecond = 1,
            HalfSecond = 2,
            OneSecond = 3,
            TwoSecond = 4
        }

        public enum MeasuremantStatus
        {
            Valid,
            AmbientLightTooHigh,
            Invalid
        }

        public class TotalHemoglobin
        {
            public MeasuremantStatus Status { get; set; }
            public double Concentration { get; set; }
        }

        public class SaturatedHemoglobin
        {
            public MeasuremantStatus Status { set; get; }
            public double PercentSaturated { get; set; }
        }

        public byte EventCount { get; private set; }
        public bool UtcTimeRequired { get; private set; }
        public bool SupportsAntFs { get; private set; }
        public MeasurementInterval Interval { get; private set; }
        public TotalHemoglobin TotalHemoglobinConcentration { get; private set; }
        public SaturatedHemoglobin PreviousSaturatedHemoglobin { get; private set; }
        public SaturatedHemoglobin CurrentSaturatedHemoglobin { get; private set; }
        public CommonDataPages CommonDataPages { get; private set; } = new CommonDataPages();


        public MuscleOxygen(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
            TotalHemoglobinConcentration = new TotalHemoglobin();
            PreviousSaturatedHemoglobin = new SaturatedHemoglobin();
            CurrentSaturatedHemoglobin = new SaturatedHemoglobin();
        }

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
