using AntRadioInterface;
using System;
using System.Linq;

namespace AntPlus.DeviceProfiles.BikeSpeedAndCadence
{
    public class CombinedSpeedAndCadenceSensor : AntDevice
    {
        /// <summary>
        /// The CombinedSpeedAndCadenceSensor device class ID.
        /// </summary>
        public const byte DeviceClass = 121;

        public event EventHandler CombinedSpeedAndCadenceSensorChanged;

        private bool isFirstDataMessage = true;     // used for accumulated values
        private ushort prevSpeedEventTime;
        private ushort prevSpeedRevCount;
        private ushort prevCadenceEventTime;
        private ushort prevCadenceRevCount;

        public double InstantaneousCadence { get; private set; }
        public double WheelCircumference { get; set; } = 2.2;
        public double InstantaneousSpeed { get; private set; }
        public double AccumulatedDistance { get; private set; }

        public CombinedSpeedAndCadenceSensor(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
        }

        public override void Parse(byte[] dataPage)
        {
            int deltaEventTime;
            int deltaRevCount;

            // ignore duplicate/unchanged data pages
            if (lastDataPage.SequenceEqual(dataPage))
            {
                return;
            }
            lastDataPage = dataPage;

            if (isFirstDataMessage)
            {
                isFirstDataMessage = false;
                prevSpeedEventTime = BitConverter.ToUInt16(dataPage, 4);
                prevCadenceEventTime = BitConverter.ToUInt16(dataPage, 0);
                prevSpeedRevCount = BitConverter.ToUInt16(dataPage, 6);
                prevCadenceRevCount = BitConverter.ToUInt16(dataPage, 2);
                lastDataPage = dataPage;
                return;
            }

            deltaEventTime = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 0), ref prevCadenceEventTime);
            if (deltaEventTime != 0)
            {
                deltaRevCount = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 2), ref prevCadenceRevCount);
                InstantaneousCadence = 60.0 * deltaRevCount * 1024.0 / deltaEventTime;
            }

            deltaEventTime = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 4), ref prevSpeedEventTime);
            if (deltaEventTime != 0)
            {
                deltaRevCount = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 6), ref prevSpeedRevCount);
                InstantaneousSpeed = WheelCircumference * deltaRevCount * 1024.0 / deltaEventTime;
                AccumulatedDistance += WheelCircumference * deltaRevCount;
            }
            CombinedSpeedAndCadenceSensorChanged?.Invoke(this, EventArgs.Empty);
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
