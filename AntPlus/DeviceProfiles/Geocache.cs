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

        public string TrackableId { get; set; }
        public uint ProgrammingPIN { get; private set; }
        public byte TotalPagesProgrammed { get; private set; }
        public uint NextStageLatitude { get; private set; }
        public uint NextStageLongitude { get; private set; }
        public string Hint { get; private set; } = string.Empty;
        public ushort NumberOfVisits { get; private set; }
        public int LastVisitTimestamp { get; private set; }
        public CommonDataPages CommonDataPages { get; private set; }

        public event EventHandler HintChanged;
        public event EventHandler LoggedVisitsChanged;
        public event EventHandler PinPageChanged;

        public Geocache(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
        }

        public override void Parse(byte[] dataPage)
        {
            switch ((DataPage)dataPage[0])
            {
                case DataPage.TrackableId:
                    TrackableId = ParseId(dataPage);
                    break;
                case DataPage.PIN:
                    ProgrammingPIN = BitConverter.ToUInt32(dataPage, 2);
                    TotalPagesProgrammed = dataPage[6];
                    PinPageChanged?.Invoke(this, new EventArgs());
                    break;
                case DataPage.AuthenticationPage:
                    break;
                default:
                    if (dataPage[0] >= 2 || dataPage[0] <= 31)
                    {
                        switch ((DataId)dataPage[1])
                        {
                            case DataId.Latitude:
                                NextStageLatitude = BitConverter.ToUInt32(dataPage, 2);
                                break;
                            case DataId.Longitude:
                                NextStageLongitude = BitConverter.ToUInt32(dataPage, 2);
                                break;
                            case DataId.Hint:
                                ParseHint(dataPage);
                                break;
                            case DataId.LoggedVisits:
                                LastVisitTimestamp = BitConverter.ToInt32(dataPage, 2);
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
                firstHintPage = dataPage[0];
                Hint = Encoding.UTF8.GetString(dataPage, 2, 6).TrimEnd((char)0);
                HintChanged?.Invoke(this, EventArgs.Empty);
            }
            else if (dataPage[0] < firstHintPage)
            {
                // start over if we received later hint pages first
                firstHintPage = dataPage[0];
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

        public void RequestAuthentication()
        {

        }

        public void UpdateLoggedVisits()
        {

        }

        public void ProgramGeocache()
        {

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
