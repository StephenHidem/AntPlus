﻿using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallEarthTech.AntPlus.DeviceProfiles
{
    /// <summary>
    /// This class supports geocaches. This is specified in ANT+ Managed Network Document – Geocache Device Profile.
    /// </summary>
    /// <remarks>
    /// Latitude and longitude coordinates are transmitted as 
    /// </remarks>
    /// <seealso cref="AntDevice" />
    public partial class Geocache : AntDevice
    {
        /// <summary>
        /// The device type value transmitted in the channel ID.
        /// </summary>
        public const byte DeviceClass = 19;

        /// <inheritdoc/>
        public override int ChannelCount => 65535;

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
        private byte? loggedVisitsPage;

        /// <summary>Gets the trackable identifier.</summary>
        [ObservableProperty]
        private string trackableId = string.Empty;
        /// <summary>Gets the programming PIN.</summary>
        [ObservableProperty]
        private uint? programmingPIN;
        /// <summary>Gets the total pages programmed.</summary>
        [ObservableProperty]
        private byte? totalPagesProgrammed;
        /// <summary>Gets the next stage latitude in decimal degrees. North is positive, south is negative.</summary>
        [ObservableProperty]
        private double nextStageLatitude;
        /// <summary>Gets the next stage longitude in decimal degrees. East is positive, west is negative.</summary>
        [ObservableProperty]
        private double nextStageLongitude;
        /// <summary>Gets a message from the geocache device, or a next stage hint.</summary>
        [ObservableProperty]
        private string hint = string.Empty;
        /// <summary>Gets the number of visits logged.</summary>
        [ObservableProperty]
        private ushort? numberOfVisits;
        /// <summary>Gets the last visit timestamp.</summary>
        [ObservableProperty]
        private DateTime? lastVisitTimestamp;
        /// <summary>Gets the authentication token.</summary>
        [ObservableProperty]
        private byte[]? authenticationToken;

        /// <summary>Gets the common data pages.</summary>
        public CommonDataPages CommonDataPages { get; private set; }
        /// <inheritdoc/>
        public override Stream DeviceImageStream => typeof(Geocache).Assembly.GetManifestResourceStream("SmallEarthTech.AntPlus.Images.Geocache.png");

        /// <summary>
        /// Initializes a new instance of the <see cref="Geocache"/> class.
        /// </summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, int)"/>
        /// <remarks>
        /// The geocache typically broadcasts its presence at 0.5Hz. The geocache changes its message rate to 4Hz upon
        /// receiving a request such as <see cref="RequestPinPage"/>. Set the timeout appropriate for your use case.
        /// </remarks>
        public Geocache(ChannelId channelId, IAntChannel antChannel, ILogger<Geocache> logger, int timeout)
            : base(channelId, antChannel, logger, timeout)
        {
            CommonDataPages = new CommonDataPages(logger);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Geocache"/> class.
        /// </summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, TimeoutOptions?)"/>
        /// <remarks>
        /// The geocache typically broadcasts its presence at 0.5Hz. The geocache changes its message rate to 4Hz upon
        /// receiving a request such as <see cref="RequestPinPage"/>. Set the timeout appropriate for your use case.
        /// </remarks>
        public Geocache(ChannelId channelId, IAntChannel antChannel, ILogger<Geocache> logger, TimeoutOptions? timeoutOptions)
            : base(channelId, antChannel, logger, timeoutOptions)
        {
            CommonDataPages = new CommonDataPages(logger);
        }

        /// <inheritdoc/>
        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);

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
                    uint pin = BitConverter.ToUInt32(dataPage, 2);
                    if (pin != 0xFFFFFFFF)
                    {
                        ProgrammingPIN = pin;
                    }
                    if (dataPage[6] != 0xFF)
                    {
                        TotalPagesProgrammed = dataPage[6];
                    }
                    break;
                case DataPage.AuthenticationPage:
                    if (authRequested)
                    {
                        authRequested = false;
                        AuthenticationToken = dataPage.Skip(1).ToArray();
                    }
                    break;
                default:
                    if (dataPage[0] >= 2 && dataPage[0] <= 31)
                    {
                        switch ((DataId)dataPage[1])
                        {
                            case DataId.Latitude:
                                NextStageLatitude = Utils.SemicirclesToDegrees(BitConverter.ToInt32(dataPage, 2));
                                break;
                            case DataId.Longitude:
                                NextStageLongitude = Utils.SemicirclesToDegrees(BitConverter.ToInt32(dataPage, 2));
                                break;
                            case DataId.Hint:
                                ParseHint(dataPage);
                                break;
                            case DataId.LoggedVisits:
                                loggedVisitsPage = dataPage[0];
                                NumberOfVisits = BitConverter.ToUInt16(dataPage, 6);
                                if (NumberOfVisits > 0)
                                {
                                    LastVisitTimestamp = new DateTime(1989, 12, 31) + TimeSpan.FromSeconds(BitConverter.ToUInt32(dataPage, 2));
                                }
                                else { LastVisitTimestamp = null; }
                                break;
                            default:
                                _logger.LogUnknownDataPage<DataId>(dataPage[1], dataPage);
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
                Hint = string.Concat(hintPages.Values);
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
        /// <returns>Status of the request.</returns>
        public async Task<MessagingReturnCode> RequestPinPage()
        {
            _logger.LogMethodEntry();
            ClearGeocacheState();
            return await RequestDataPage(DataPage.PIN);
        }

        /// <summary>Requests the authentication token from the geocache.</summary>
        /// <param name="gpsSerialNumber">The GPS serial number.</param>
        /// <returns>Status of the request.</returns>
        public async Task<MessagingReturnCode> RequestAuthentication(uint gpsSerialNumber)
        {
            _logger.LogMethodEntry();
            authRequested = true;
            Random random = new Random();
            byte[] nonce = new byte[2];
            random.NextBytes(nonce);
            byte[] msg = { (byte)DataPage.AuthenticationPage, 0xFF };
            msg = msg.Concat(nonce).Concat(BitConverter.GetBytes(gpsSerialNumber)).ToArray();
            return await SendExtAcknowledgedMessage(msg);
        }

        /// <summary>Updates the logged visits count and last visit timestamp.</summary>
        /// <exception cref="InvalidOperationException">The geocache has not been programmed.</exception>
        /// <returns>Status of the request.</returns>
        public async Task<MessagingReturnCode> UpdateLoggedVisits()
        {
            _logger.LogMethodEntry();
            // check that a logged visits page has been programmed
            if (loggedVisitsPage == null || NumberOfVisits == null)
            {
                throw new InvalidOperationException("The geocache has not been programmed with a logged visits page. Program the geocache to set a logged visits page.");
            }
            NumberOfVisits++;
            LastVisitTimestamp = DateTime.UtcNow;
            uint timestamp = (uint)(DateTime.UtcNow - new DateTime(1989, 12, 31)).TotalSeconds;
            return await SendExtAcknowledgedMessage(new byte[] { (byte)loggedVisitsPage, (byte)DataId.LoggedVisits }.
                Concat(BitConverter.GetBytes(timestamp)).
                Concat(BitConverter.GetBytes((ushort)NumberOfVisits)).ToArray());
        }

        /// <summary>Programs the geocache.</summary>
        /// <param name="id">The trackable ID. Must be less than or equal to 9 characters.</param>
        /// <param name="pin">The programming PIN.</param>
        /// <param name="latitude">The latitude in decimal degrees.</param>
        /// <param name="longitude">The longitude in decimal degrees.</param>
        /// <param name="hint">The next stage hint or message.</param>
        /// <returns>Status of the request.</returns>
        public async Task<MessagingReturnCode> ProgramGeocache(string id, uint pin, double? latitude, double? longitude, string? hint)
        {
            _logger.LogMethodEntry();
            programmingGeocache = true;
            List<byte[]> messages = new List<byte[]>();

            // get the previous total number of pages programmed
            int previousPagesProgrammed = TotalPagesProgrammed ?? 0;

            // clear previous Geocache state
            ClearGeocacheState();

            // add ID page to messages
            messages.Add(new byte[] { (byte)DataPage.TrackableId }.Concat(FormatId(id)).ToArray());

            // add logged visits page to 0 visits and no timestamp to messages
            byte page = 2;  // initial page number for optional pages
            messages.Add(new byte[] { page++, (byte)DataId.LoggedVisits, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00 });

            // add optional latitude page to messages
            if (latitude != null)
            {
                messages.Add(new byte[] { page++, (byte)DataId.Latitude }.
                            Concat(BitConverter.GetBytes(Utils.DegreesToSemicircles((double)latitude))).
                            Concat(new byte[] { 0xFF, 0xFF }).ToArray());
            }

            // add optional longitude page to messages
            if (longitude != null)
            {
                messages.Add(new byte[] { page++, (byte)DataId.Longitude }.
                        Concat(BitConverter.GetBytes(Utils.DegreesToSemicircles((double)longitude))).
                        Concat(new byte[] { 0xFF, 0xFF }).ToArray());
            }

            // add optional hint pages to messages
            if (!string.IsNullOrEmpty(hint))
            {
                // get hint and pad with null terminator
                byte[] hnt = Encoding.UTF8.GetBytes(hint!.PadRight(hint.Length + 6 - (hint.Length % 6), '\0'));
                while (hnt.Length > 0 && page < 32)
                {
                    messages.Add(new byte[] { page++, (byte)DataId.Hint }.Concat(hnt.Take(6)).ToArray());
                    hnt = hnt.Skip(6).ToArray();
                }
            }

            // add PIN page to messages
            messages.Add(new byte[] { (byte)DataPage.PIN, 0xFF }.
                    Concat(BitConverter.GetBytes((uint)pin)).
                    Concat(new byte[] { page }).        // set the total number of pages programmed
                    Concat(new byte[] { 0xFF }).
                    ToArray());

            // add messages to clear previously programmed pages
            for (int i = page; i <= previousPagesProgrammed; i++)
            {
                messages.Add(new byte[] { (byte)i, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
            }

            // send pages to geocache
            MessagingReturnCode returnCode = MessagingReturnCode.Pass;
            foreach (byte[] msg in messages)
            {
                returnCode = await SendExtAcknowledgedMessage(msg);
                if (returnCode != MessagingReturnCode.Pass)
                {
                    break;
                }
            }

            programmingGeocache = false;
            return returnCode;
        }

        private void ClearGeocacheState()
        {
            TrackableId = string.Empty;
            ProgrammingPIN = default;
            TotalPagesProgrammed = default;
            NextStageLatitude = NextStageLongitude = default;
            hintPages.Clear();
            Hint = string.Empty;
            NumberOfVisits = default;
            LastVisitTimestamp = default;
        }

        /// <summary>
        /// Erases the geocache.
        /// </summary>
        /// <returns>Status of the request.</returns>
        public async Task<MessagingReturnCode> EraseGeocache()
        {
            _logger.LogMethodEntry();
            programmingGeocache = true;

            // clear previous Geocache state
            ClearGeocacheState();

            // set trackable ID page to zero
            MessagingReturnCode returnCode = await SendExtAcknowledgedMessage(new byte[] { (byte)DataPage.TrackableId, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            if (returnCode != MessagingReturnCode.Pass)
            {
                programmingGeocache = false;
                return returnCode;
            }

            // erase all data pages
            for (byte i = 1; i < 32; i++)
            {
                returnCode = await SendExtAcknowledgedMessage(new byte[] { i, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
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
