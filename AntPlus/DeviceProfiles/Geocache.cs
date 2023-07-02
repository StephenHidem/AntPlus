using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SmallEarthTech.AntPlus.DeviceProfiles
{
    /// <summary>
    /// This class supports geocaches. This is specified in ANT+ Managed Network Document – Geocache Device Profile.
    /// </summary>
    /// <remarks>
    /// Latitude and longitude coordinates are transmitted as 
    /// </remarks>
    /// <seealso cref="AntDevice" />
    public class Geocache : AntDevice
    {
        /// <summary>
        /// The geocache device class ID.
        /// </summary>
        public const byte DeviceClass = 19;

        /// <summary>
        /// Main data pages.
        /// </summary>
        public enum DataPage
        {
            /// <summary>The trackable identifier</summary>
            TrackableId = 0x00,
            /// <summary>The PIN number</summary>
            PIN = 0x01,
            /// <summary>The authentication page</summary>
            AuthenticationPage = 0x20,
        }

        /// <summary>Data identifier of the geocache data page.</summary>
        private enum DataId
        {
            /// <summary>Latitude</summary>
            Latitude,
            /// <summary>Longitude</summary>
            Longitude,
            /// <summary>Hint or message</summary>
            Hint,
            /// <summary>The number of logged visits</summary>
            LoggedVisits = 4
        }

        private bool authRequested;
        private bool programmingGeocache;
        private byte loggedVisitsPage;
        private DateTime lastMessageTime;

        /// <summary>Gets the trackable identifier.</summary>
        public string TrackableId { get; private set; }
        /// <summary>Gets the programming PIN.</summary>
        public uint? ProgrammingPIN { get; private set; }
        /// <summary>Gets the total pages programmed.</summary>
        public byte? TotalPagesProgrammed { get; private set; }
        /// <summary>Gets the next stage latitude in decimal degrees. North is positive, south is negative.</summary>
        public double? NextStageLatitude { get; private set; }
        /// <summary>Gets the next stage longitude in decimal degrees. East is positive, west is negative.</summary>
        public double? NextStageLongitude { get; private set; }
        /// <summary>Gets a message from the geocache device, or a next stage hint.</summary>
        public string Hint => string.Concat(hintPages.Values);
        /// <summary>Gets the number of visits logged.</summary>
        public ushort? NumberOfVisits { get; private set; }
        /// <summary>Gets the last visit timestamp.</summary>
        public DateTime? LastVisitTimestamp { get; private set; }
        /// <summary>Gets the authentication token.</summary>
        public byte[] AuthenticationToken { get; private set; } = new byte[0];
        /// <summary>Gets the message rate in Hz.
        /// This may be used to determine if the geocache is in low power mode and broadcasting at 0.5Hz or in active mode and broadcasting at 4Hz.</summary>
        /// <value>The message rate in Hz.</value>
        public double MessageRate { get; private set; }
        /// <summary>Gets the common data pages.</summary>
        public CommonDataPages CommonDataPages { get; private set; }
        /// <inheritdoc/>
        public override Stream DeviceImageStream => typeof(Geocache).Assembly.GetManifestResourceStream("SmallEarthTech.AntPlus.Images.Geocache.png");

        /// <summary>
        /// Initializes a new instance of the <see cref="Geocache"/> class.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        /// <param name="logger">Logger to use.</param>
        /// <param name="timeout">Device offline timeout. The default is 8000 milliseconds.</param>
        /// <remarks>
        /// The geocache typically broadcasts its presence at 0.5Hz. The geocache changes its message rate to 4Hz upon
        /// receiving a request such as <see cref="RequestPinPage"/>. Set the timeout appropriate for your use case.
        /// </remarks>
        public Geocache(ChannelId channelId, IAntChannel antChannel, ILogger<Geocache> logger, int timeout = 8000) : base(channelId, antChannel, logger, timeout)
        {
            CommonDataPages = new CommonDataPages(logger);
            lastMessageTime = DateTime.Now;
        }

        /// <inheritdoc/>
        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);

            // determine message rate
            DateTime now = DateTime.Now;
            MessageRate = 1.0 / now.Subtract(lastMessageTime).TotalSeconds;
            lastMessageTime = now;
            RaisePropertyChange(nameof(MessageRate));

            // don't parse if programming geocache
            if (programmingGeocache)
            {
                return;
            }

            switch ((DataPage)dataPage[0])
            {
                case DataPage.TrackableId:
                    TrackableId = ParseId(dataPage);
                    RaisePropertyChange(nameof(TrackableId));
                    break;
                case DataPage.PIN:
                    uint pin = BitConverter.ToUInt32(dataPage, 2);
                    if (pin != 0xFFFFFFFF)
                    {
                        ProgrammingPIN = pin;
                    }
                    RaisePropertyChange(nameof(ProgrammingPIN));
                    if (dataPage[6] != 0xFF)
                    {
                        TotalPagesProgrammed = dataPage[6];
                    }
                    RaisePropertyChange(nameof(TotalPagesProgrammed));
                    break;
                case DataPage.AuthenticationPage:
                    if (authRequested)
                    {
                        authRequested = false;
                        AuthenticationToken = dataPage.Skip(1).ToArray();
                        RaisePropertyChange(nameof(AuthenticationToken));
                    }
                    break;
                default:
                    if (dataPage[0] >= 2 && dataPage[0] <= 31)
                    {
                        switch ((DataId)dataPage[1])
                        {
                            case DataId.Latitude:
                                NextStageLatitude = Utils.SemicirclesToDegrees(BitConverter.ToInt32(dataPage, 2));
                                RaisePropertyChange(nameof(NextStageLatitude));
                                break;
                            case DataId.Longitude:
                                NextStageLongitude = Utils.SemicirclesToDegrees(BitConverter.ToInt32(dataPage, 2));
                                RaisePropertyChange(nameof(NextStageLongitude));
                                break;
                            case DataId.Hint:
                                ParseHint(dataPage);
                                RaisePropertyChange(nameof(Hint));
                                break;
                            case DataId.LoggedVisits:
                                loggedVisitsPage = dataPage[0];
                                NumberOfVisits = BitConverter.ToUInt16(dataPage, 6);
                                RaisePropertyChange(nameof(NumberOfVisits));
                                if (NumberOfVisits > 0)
                                {
                                    LastVisitTimestamp = new DateTime(1989, 12, 31) + TimeSpan.FromSeconds(BitConverter.ToUInt32(dataPage, 2));
                                }
                                else { LastVisitTimestamp = null; }
                                RaisePropertyChange(nameof(LastVisitTimestamp));
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

        private readonly SortedList<byte, string> hintPages = new SortedList<byte, string>();
        private void ParseHint(byte[] dataPage)
        {
            if (!hintPages.ContainsKey(dataPage[0]))
            {
                hintPages.Add(dataPage[0], Encoding.UTF8.GetString(dataPage, 2, 6).TrimEnd((char)0));
            }
        }

        private string ParseId(byte[] dataPage)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append((char)((dataPage[1] >> 2) + 0x20));
            stringBuilder.Append((char)((Utils.RotateLeft(BitConverter.ToUInt16(dataPage, 1), 4) & 0x3F) + 0x20));
            stringBuilder.Append((char)((Utils.RotateLeft(BitConverter.ToUInt16(dataPage, 2), 2) & 0x3F) + 0x20));
            stringBuilder.Append((char)((dataPage[3] & 0x3F) + 0x20));
            stringBuilder.Append((char)((dataPage[4] >> 2) + 0x20));
            stringBuilder.Append((char)((Utils.RotateLeft(BitConverter.ToUInt16(dataPage, 4), 4) & 0x3F) + 0x20));
            stringBuilder.Append((char)((Utils.RotateLeft(BitConverter.ToUInt16(dataPage, 5), 2) & 0x3F) + 0x20));
            stringBuilder.Append((char)((dataPage[6] & 0x3F) + 0x20));
            stringBuilder.Append((char)((dataPage[7] >> 2) + 0x20));
            return stringBuilder.ToString();
        }

        private byte[] FormatId(string id)
        {
            // pad ID with spaces if less than 9 characters
            id = id.PadRight(9, ' ');

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
            return BitConverter.GetBytes(ch1).Take(3).Reverse().Concat(BitConverter.GetBytes(ch2).Reverse()).ToArray();
        }

        /// <summary>Requests the PIN page. Do this first before performing any other operations on the geocache.</summary>
        /// <param name="timeout">Request timeout in milliseconds. The default is 16000 milliseconds.</param>
        /// <returns>Status of the request. See <see cref="MessagingReturnCode" />.</returns>
        public MessagingReturnCode RequestPinPage(uint timeout = 16000)
        {
            // clear any previous state
            TrackableId = string.Empty;
            ProgrammingPIN = default;
            TotalPagesProgrammed = default;
            NextStageLatitude = NextStageLongitude = default;
            hintPages.Clear();
            NumberOfVisits = default;
            LastVisitTimestamp = default;
            RaisePropertyChange(string.Empty);

            return RequestDataPage(DataPage.PIN, timeout);
        }

        /// <summary>Requests the authentication.</summary>
        /// <param name="gpsSerialNumber">The GPS serial number.</param>
        /// <returns>Status of the request. See <see cref="MessagingReturnCode"/></returns>
        public MessagingReturnCode RequestAuthentication(uint gpsSerialNumber)
        {
            authRequested = true;
            Random random = new Random();
            byte[] nonce = new byte[2];
            random.NextBytes(nonce);
            byte[] msg = { (byte)DataPage.AuthenticationPage, 0xFF };
            msg = msg.Concat(nonce).Concat(BitConverter.GetBytes(gpsSerialNumber)).ToArray();
            return SendExtAcknowledgedMessage(msg, 16000);
        }

        /// <summary>Updates the logged visits count and last visit timestamp.</summary>
        /// <exception cref="InvalidOperationException">The geocache has not been programmed.</exception>
        /// <returns>Status of the request. See <see cref="MessagingReturnCode"/>.</returns>
        public MessagingReturnCode UpdateLoggedVisits()
        {
            // check that a logged visits page has been programmed
            if (loggedVisitsPage == 0)
            {
                throw new InvalidOperationException("The geocache has not been programmed with a logged visits pages. Program the geocache; this will set a logged visits page.");
            }
            NumberOfVisits++;
            LastVisitTimestamp = DateTime.UtcNow;
            uint timestamp = (uint)(DateTime.UtcNow - new DateTime(1989, 12, 31)).TotalSeconds;
            return SendExtAcknowledgedMessage(new byte[] { loggedVisitsPage, (byte)DataId.LoggedVisits }.
                Concat(BitConverter.GetBytes(timestamp)).
                Concat(BitConverter.GetBytes((short)NumberOfVisits)).ToArray(), 16000);
        }

        /// <summary>Programs the geocache.</summary>
        /// <param name="id">The trackable ID. Must be less than or equal to 9 characters.</param>
        /// <param name="pin">The programming PIN.</param>
        /// <param name="latitude">The latitude in decimal degrees.</param>
        /// <param name="longitude">The longitude in decimal degrees.</param>
        /// <param name="hint">The next stage hint or message.</param>
        /// <returns>Status of the request. See <see cref="MessagingReturnCode"/>.</returns>
        public MessagingReturnCode ProgramGeocache(string id, uint? pin, double? latitude, double? longitude, string hint)
        {
            programmingGeocache = true;
            byte page = 1;  // initial page number for optional pages

            // clear any previous state
            TrackableId = string.Empty;
            ProgrammingPIN = default;
            TotalPagesProgrammed = default;
            NextStageLatitude = NextStageLongitude = default;
            hintPages.Clear();
            NumberOfVisits = default;
            LastVisitTimestamp = default;
            RaisePropertyChange(string.Empty);

            // set ID to empty string if null
            if (id is null)
            {
                id = string.Empty;
            }

            // assemble list of messages to send; simplifies error handling
            List<byte[]> messages = new List<byte[]>
            {
                // ID page
                new byte[] { (byte)DataPage.TrackableId }.Concat(FormatId(id)).ToArray(),
            };

            // program optional pages and PIN if PIN is not null
            if (pin != null)
            {
                page = 2;   // optional pages begin at page 2

                // initialize logged visits page to 0 visits and no timestamp
                messages.Add(new byte[] { page++, (byte)DataId.LoggedVisits, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00 });

                // optional latitude page
                if (latitude != null)
                {
                    messages.Add(new byte[] { page++, (byte)DataId.Latitude }.
                                Concat(BitConverter.GetBytes(Utils.DegreesToSemicircles((double)latitude))).
                                Concat(new byte[] { 0xFF, 0xFF }).ToArray());
                }

                // optional longitude page
                if (longitude != null)
                {
                    messages.Add(new byte[] { page++, (byte)DataId.Longitude }.
                            Concat(BitConverter.GetBytes(Utils.DegreesToSemicircles((double)longitude))).
                            Concat(new byte[] { 0xFF, 0xFF }).ToArray());
                }

                // optional hint pages - get hint and pad with null terminator
                if (hint?.Length > 0)
                {
                    byte[] hnt = Encoding.UTF8.GetBytes(hint.PadRight(hint.Length + 6 - (hint.Length % 6), '\0'));
                    while (hnt.Length > 0 && page < 32)
                    {
                        messages.Add(new byte[] { page++, (byte)DataId.Hint }.Concat(hnt.Take(6)).ToArray());
                        hnt = hnt.Skip(6).ToArray();
                    }
                }

                // finally, program PIN page and total pages programmed
                messages.Add(new byte[] { (byte)DataPage.PIN, 0xFF }.
                        Concat(BitConverter.GetBytes((uint)pin)).
                        Concat(new byte[] { page }).
                        Concat(new byte[] { 0xFF }).
                        ToArray());
            }

            // clear remaining unused programmable pages
            while (page < 32)
            {
                messages.Add(new byte[] { page++, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
            }

            // send pages to geocache
            MessagingReturnCode returnCode = MessagingReturnCode.Pass;
            foreach (byte[] msg in messages)
            {
                returnCode = SendExtAcknowledgedMessage(msg, 16000);
                if (returnCode != MessagingReturnCode.Pass)
                {
                    break;
                }
            }

            programmingGeocache = false;
            return returnCode;
        }
    }
}
