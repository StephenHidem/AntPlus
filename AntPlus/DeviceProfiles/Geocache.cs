using AntRadioInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AntPlus.DeviceProfiles
{
    public class Geocache : AntDevice
    {
        /// The fitness equipment device class ID.
        /// </summary>
        public const byte DeviceClass = 19;

        /// <summary>
        /// Main data pages.
        /// </summary>
        public enum DataPage
        {
            TrackableId = 0x00,
            PIN = 0x01,
            AuthenticationPage = 0x20,
        }

        public enum DataId
        {
            Latitude,
            Longitude,
            Hint,
            LoggedVisits = 4
        }

        private byte firstHintPage;
        private byte lastHintPage;
        private bool authRequested;
        private bool programmingGeocache;
        private byte loggedVisitsPage;

        public string TrackableId { get; set; }
        public uint ProgrammingPIN { get; private set; }
        public byte TotalPagesProgrammed { get; private set; }
        public uint NextStageLatitude { get; private set; }
        public uint NextStageLongitude { get; private set; }
        public string Hint { get; private set; } = string.Empty;
        public ushort NumberOfVisits { get; private set; }


        public DateTime LastVisitTimestamp { get; private set; }
        public byte[] AuthenticationToken { get; set; }
        public CommonDataPages CommonDataPages { get; private set; }

        public event EventHandler HintChanged;
        public event EventHandler LoggedVisitsChanged;
        public event EventHandler PinPageChanged;
        public event EventHandler AuthenticationPageChanged;
        public event EventHandler LatitudePageChanged;
        public event EventHandler LongitudePageChanged;

        public Geocache(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
        }

        public override void Parse(byte[] dataPage)
        {
            // don't parse if programming geocache
            if (programmingGeocache)
            {
                return;
            }

            switch ((DataPage)dataPage[0])
            {
                case DataPage.TrackableId:
                    TrackableId = ParseId(dataPage);
                    break;
                case DataPage.PIN:
                    ProgrammingPIN = BitConverter.ToUInt32(dataPage, 2);
                    TotalPagesProgrammed = dataPage[6];
                    PinPageChanged?.Invoke(this, EventArgs.Empty);
                    break;
                case DataPage.AuthenticationPage:
                    if (authRequested)
                    {
                        authRequested = false;
                        AuthenticationToken = dataPage.Skip(1).ToArray();
                        AuthenticationPageChanged?.Invoke(this, EventArgs.Empty);
                    }
                    break;
                default:
                    if (dataPage[0] >= 2 || dataPage[0] <= 31)
                    {
                        switch ((DataId)dataPage[1])
                        {
                            case DataId.Latitude:
                                NextStageLatitude = BitConverter.ToUInt32(dataPage, 2);
                                LatitudePageChanged?.Invoke(this, EventArgs.Empty);
                                break;
                            case DataId.Longitude:
                                NextStageLongitude = BitConverter.ToUInt32(dataPage, 2);
                                LongitudePageChanged?.Invoke(this, EventArgs.Empty);
                                break;
                            case DataId.Hint:
                                ParseHint(dataPage);
                                break;
                            case DataId.LoggedVisits:
                                loggedVisitsPage = dataPage[0];
                                LastVisitTimestamp = new DateTime(1989, 12, 31) + TimeSpan.FromSeconds(BitConverter.ToUInt32(dataPage, 2));
                                NumberOfVisits = BitConverter.ToUInt16(dataPage, 6);
                                LoggedVisitsChanged?.Invoke(this, EventArgs.Empty);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        CommonDataPages.ParseCommonDataPage(dataPage);
                    }
                    break;
            }
        }

        private void ParseHint(byte[] dataPage)
        {
            if (firstHintPage == 0)
            {
                // initial hint received, could be any datapage
                firstHintPage = lastHintPage = dataPage[0];
                Hint = Encoding.UTF8.GetString(dataPage, 2, 6).TrimEnd((char)0);
                HintChanged?.Invoke(this, EventArgs.Empty);
            }
            else if (dataPage[0] < firstHintPage)
            {
                // start over if we received later hint pages first
                firstHintPage = lastHintPage = dataPage[0];
                Hint = Encoding.UTF8.GetString(dataPage, 2, 6).TrimEnd((char)0);
                HintChanged?.Invoke(this, EventArgs.Empty);
            }
            else if (dataPage[0] > lastHintPage)
            {
                // append subsequent hint pages
                lastHintPage = dataPage[0];
                Hint += Encoding.UTF8.GetString(dataPage, 2, 6).TrimEnd((char)0);
                HintChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private string ParseId(byte[] dataPage)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append((char)((dataPage[1] >> 2) + 0x20));
            stringBuilder.Append((char)((RotateLeft(BitConverter.ToUInt16(dataPage, 1), 4) & 0x3F) + 0x20));
            stringBuilder.Append((char)((RotateLeft(BitConverter.ToUInt16(dataPage, 2), 2) & 0x3F) + 0x20));
            stringBuilder.Append((char)((dataPage[3] & 0x3F) + 0x20));
            stringBuilder.Append((char)((dataPage[4] >> 2) + 0x20));
            stringBuilder.Append((char)((RotateLeft(BitConverter.ToUInt16(dataPage, 4), 4) & 0x3F) + 0x20));
            stringBuilder.Append((char)((RotateLeft(BitConverter.ToUInt16(dataPage, 5), 2) & 0x3F) + 0x20));
            stringBuilder.Append((char)((dataPage[6] & 0x3F) + 0x20));
            stringBuilder.Append((char)((dataPage[7] >> 2) + 0x20));
            return stringBuilder.ToString();
        }

        private byte[] FormatId(string id)
        {
            // pad ID with spaces if less than 9 characters
            if (id.Length < 9)
            {
                id = id.PadRight(9, ' ');
            }
            id = id.ToUpper();
            int ch1 = (id[0] - 0x20) << 18;
            ch1 |= ((id[1] - 0x20)) << 12;
            ch1 |= ((id[2] - 0x20)) << 6;
            ch1 |= (id[3] - 0x20);
            int ch2 = (id[4] - 0x20) << 26;
            ch2 |= (id[5] - 0x20) << 20;
            ch2 |= (id[6] - 0x20) << 14;
            ch2 |= (id[7] - 0x20) << 8;
            ch2 |= (id[8] - 0x20) << 2;
            return BitConverter.GetBytes(ch1).Take(3).Concat(BitConverter.GetBytes(ch2).Reverse()).ToArray();
        }

        private ushort RotateLeft(ushort value, int rotate)
        {
            return (ushort)((value << rotate) | (value >> (16 - rotate)));
        }

        private ushort RotateRight(ushort value, int rotate)
        {
            return (ushort)((value >> rotate) | (value << (16 - rotate)));
        }

        public void RequestPinPage()
        {
            RequestDataPage(DataPage.PIN);
        }

        public void RequestAuthentication(uint gpsSerialNumber)
        {
            authRequested = true;
            Random random = new Random();
            byte[] nonce = new byte[2];
            random.NextBytes(nonce);
            byte[] msg = { (byte)DataPage.AuthenticationPage, 0xFF };
            msg = msg.Concat(nonce).Concat(BitConverter.GetBytes(gpsSerialNumber)).ToArray();
            SendExtAcknowledgedMessage(msg);
        }

        public void UpdateLoggedVisits()
        {
            ushort addVisit = (ushort)(NumberOfVisits + 1);
            uint timestamp = (uint)(DateTime.UtcNow - new DateTime(1989, 12, 31)).TotalSeconds;
            SendExtAcknowledgedMessage(new byte[] { loggedVisitsPage, (byte)DataId.LoggedVisits }.
                Concat(BitConverter.GetBytes(timestamp)).
                Concat(BitConverter.GetBytes(addVisit)).ToArray());
        }

        public void ProgramGeocache(string id, uint pin, uint latitude, uint longitude, string hint)
        {
            programmingGeocache = true;

            byte[] msg;

            // get hint and pad with null terminator
            byte[] hnt = Encoding.UTF8.GetBytes(hint.PadRight(hint.Length + 6 - hint.Length % 6, '\0'));
            // convert to list of pages
            int i = 0;
            List<byte[]> hntPages = hnt.GroupBy(s => i++ / 6).Select(s => s.ToArray()).ToList();

            // program ID page
            msg = new byte[] { (byte)DataPage.TrackableId };
            msg = msg.Concat(FormatId(id)).ToArray();
            SendExtAcknowledgedMessage(msg);

            // program PIN page and total pages programmed
            msg = new byte[] { (byte)DataPage.PIN, 0xFF };
            msg = msg.Concat(BitConverter.GetBytes(pin)).Concat(BitConverter.GetBytes(hntPages.Count + 5).Take(2)).ToArray();
            SendExtAcknowledgedMessage(msg);

            // program logged visits - clear
            SendExtAcknowledgedMessage(new byte[] { 0x02, (byte)DataId.LoggedVisits, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00 });

            // program latitude
            msg = new byte[] { 0x03, (byte)DataId.Latitude }.
                Concat(BitConverter.GetBytes(latitude)).Concat(new byte[] { 0xFF, 0xFF }).ToArray();
            SendExtAcknowledgedMessage(msg);

            // program longitude
            msg = new byte[] { 0x04, (byte)DataId.Longitude }.
                Concat(BitConverter.GetBytes(longitude)).Concat(new byte[] { 0xFF, 0xFF }).ToArray();
            SendExtAcknowledgedMessage(msg);

            // program hint pages
            byte pageNumber = 5;
            foreach (var item in hntPages)
            {
                msg = new byte[] { pageNumber, (byte)DataId.Hint }.Concat(item).ToArray();
                SendExtAcknowledgedMessage(msg);
                pageNumber++;
            }

            // clear remaining unused programmable pages
            msg = new byte[] { pageNumber, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            while (msg[0] < 32)
            {
                SendExtAcknowledgedMessage(msg);
                msg[0]++;
            }

            programmingGeocache = false;
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
