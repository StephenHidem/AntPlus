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
            StoopSession,
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

        public bool UtcTimeRequired { get; private set; }
        public bool SupportsAntFs { get; private set; }
        public MeasurementInterval Interval { get; private set; }
        public TotalHemoglobin TotalHemoglobinConcentration { get; private set; }
        public SaturatedHemoglobin PreviousSaturatedHemoglobin { get; private set; }
        public SaturatedHemoglobin CurrentSaturatedHemoglobin { get; private set; }
        public CommonDataPages CommonDataPages { get; private set; }

        public event EventHandler MuscleOxygenChanged;

        public MuscleOxygen(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
            TotalHemoglobinConcentration = new TotalHemoglobin();
            PreviousSaturatedHemoglobin = new SaturatedHemoglobin();
            CurrentSaturatedHemoglobin = new SaturatedHemoglobin();
            CommonDataPages = new CommonDataPages();
        }

        public override void Parse(byte[] dataPage)
        {
            int val;

            switch ((DataPage)dataPage[0])
            {
                case DataPage.MuscleOxygenData:
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

                    val = (BitConverter.ToUInt16(dataPage, 5) & 0x3FF0) >> 4;
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

                    val = (BitConverter.ToUInt16(dataPage, 6) & 0xFFC0) >> 6;
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

                    MuscleOxygenChanged?.Invoke(this, EventArgs.Empty);
                    break;
                default:
                    CommonDataPages.ParseCommonDataPage(dataPage);
                    break;
            }
        }

        public void SendCommand(CommandId command, DateTimeOffset localTimeOffest, DateTime currentTimeStamp)
        {
            sbyte offset = (sbyte)(localTimeOffest.Minute / 4);
            int current = (int)(currentTimeStamp - new DateTime(1989, 12, 31)).TotalSeconds;
            byte[] msg = { (byte)DataPage.Commands, 0xFF, (byte)command, (byte)offset };
            msg.Concat(BitConverter.GetBytes(current));
            SendExtAcknowledgedMessage(msg);
        }

        public override void ChannelEventHandler(EventMsgId eventMsgId)
        {
            throw new NotImplementedException();
        }

        public override void ChannelResponseHandler(byte messageId, ResponseMsgId responseMsgId)
        {
            throw new NotImplementedException();
        }
    }
}
