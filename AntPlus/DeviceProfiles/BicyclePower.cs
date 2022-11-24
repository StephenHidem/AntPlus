using AntPlus;
using AntPlus.DeviceProfiles;
using AntRadioInterface;
using System;
using System.Linq;

namespace DeviceProfiles
{
    public class BicyclePower : AntDevice
    {
        public const byte DeviceClass = 11;

        public enum DataPage
        {
            Unknown,
            Calibration,
            GetSetParameters,
            MeasurementOutput,
            PowerOnly = 0x10,
            WheelTorque,
            CrankTorque,
            TorqueEffectivenessAndPedalSmoothness,
            TorqueBarycenter,
            CrankTorqueFrequency = 0x20,
            RightForceAngle = 0xE0,
            LeftForceAngle,
            PedalPosition
        }



        // Standard Power-Only Main Data Page (0x10)

        public StandardPowerOnly PowerOnlySensor { get; private set; }
        public StandardWheelTorqueSensor WheelTorqueSensor { get; private set; }
        public StandardCrankTorqueSensor CrankTorqueSensor { get; private set; }


        public BicyclePower(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
            // since we don't know the sensor type we create all
            PowerOnlySensor = new StandardPowerOnly(channelId, antChannel);
            WheelTorqueSensor = new StandardWheelTorqueSensor(channelId, antChannel);
            CrankTorqueSensor = new StandardCrankTorqueSensor(channelId, antChannel);
        }

        public override void Parse(byte[] dataPage)
        {
            // ignore duplicate/unchanged data pages
            if (lastDataPage.SequenceEqual(dataPage))
            {
                return;
            }
            lastDataPage = dataPage;

            switch ((DataPage)dataPage[0])
            {
                case DataPage.Unknown:
                    break;
                case DataPage.Calibration:
                    break;
                case DataPage.GetSetParameters:
                    break;
                case DataPage.MeasurementOutput:
                    break;
                case DataPage.PowerOnly:
                    PowerOnlySensor.Parse(dataPage);
                    break;
                case DataPage.WheelTorque:
                    WheelTorqueSensor.Parse(dataPage);
                    break;
                case DataPage.CrankTorque:
                    CrankTorqueSensor.Parse(dataPage);
                    break;
                case DataPage.TorqueEffectivenessAndPedalSmoothness:
                    break;
                case DataPage.CrankTorqueFrequency:
                    break;
                case DataPage.RightForceAngle:
                    break;
                case DataPage.LeftForceAngle:
                    break;
                case DataPage.PedalPosition:
                    break;
                default:
                    break;
            }
        }

        //private double ComputeAveAngularVelocity()
        //{
        //    return 2 * Math.PI * (AccumulatedEventCount - previousAccumulatedEventCount) / ((AccumulatedPeriod - previousAccumulatedPeriod) / 2048);
        //}

        //private double ComputeAveTorque()
        //{
        //    return (AccumulatedTorque - previousAccumulatedTorque) / (32 * (AccumulatedEventCount - previousAccumulatedEventCount));
        //}

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
