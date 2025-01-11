using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;

namespace SmallEarthTech.AntPlus.DeviceProfiles
{
    public partial class BikeRadar : AntDevice
    {
        #region DeviceClass and Overrides
        public const byte DeviceClass = 40;		// this value comes from the Bike Radar device type

        public override int ChannelCount => 4084;   // from master channel period in Bike Radar spec

        public override Stream? DeviceImageStream => GetType().Assembly.GetManifestResourceStream("MauiAntGrpcClient.CustomAntDevice.BikeRadar.png");

        public override string ToString() => "Bike Radar";
        #endregion

        #region DataPages
        /// <summary>Bike radar data pages.</summary>
        public enum DataPage
        {
            Unknown = 0,
            /// <summary>Status</summary>
            DeviceStatus = 1,
            /// <summary>Commend</summary>
            DeviceCommand = 2,
            /// <summary>Page A: targets 1 - 4</summary>
            RadarTargetsA = 48,
            /// <summary>Page B: targets 5 - 8</summary>
            RadarTargetsB = 49,
        }
        #endregion

        #region Custom Properties
        public enum DeviceState
        {
            Broadcasting,
            ShutdownRequested,
            ShutdownAborted,
            ShutdownForced
        }

        public enum ThreatLevel
        {
            None,
            Approaching,
            FastApproach,
            Reserved
        }

        public enum ThreatSide
        {
            Behind,
            Right,
            Left,
            Reserved
        }

        public partial class RadarTarget : ObservableObject
        {
            [ObservableProperty]
            private ThreatLevel threatLevel;
            [ObservableProperty]
            private ThreatSide threatSide;
            [ObservableProperty]
            private double range;
            [ObservableProperty]
            private double closingSpeed;
        }

        [ObservableProperty]
        private DeviceState state;
        [ObservableProperty]
        private bool clearTargets;
        public List<RadarTarget> RadarTargets { get; } = [];
        public CommonDataPages CommonDataPages { get; }
        #endregion

        #region Ctor
        public BikeRadar(ChannelId channelId, IAntChannel antChannel, ILogger<BikeRadar> logger, TimeoutOptions? options)
            : base(channelId, antChannel, logger, options)
        {
            CommonDataPages = new CommonDataPages(logger);
            for (int i = 0; i < 8; i++) { RadarTargets.Add(new RadarTarget()); }
        }
        #endregion

        #region Parse
        public override void Parse(byte[] dataPage)
        {
            uint ranges;
            ushort speeds;

            base.Parse(dataPage);
            switch ((DataPage)dataPage[0])
            {
                case DataPage.Unknown:
                    break;
                case DataPage.DeviceStatus:
                    State = (DeviceState)(dataPage[1] & 0x03);
                    ClearTargets = (dataPage[7] & 0x01) == 0;
                    break;
                case DataPage.DeviceCommand:
                    break;
                case DataPage.RadarTargetsA:
                    ranges = BitConverter.ToUInt32(dataPage, 3) & 0x00FFFFFF;
                    speeds = BitConverter.ToUInt16(dataPage, 6);
                    for (int i = 0; i < 4; i++)
                    {
                        var threatLevel = RadarTargets[i].ThreatLevel = (ThreatLevel)((dataPage[1] >> (i * 2)) & 0x03);
                        RadarTargets[i].ThreatSide = threatLevel == ThreatLevel.None ? ThreatSide.Behind : (ThreatSide)((dataPage[2] >> (i * 2)) & 0x03);
                        RadarTargets[i].Range = threatLevel == ThreatLevel.None ? 0 : 3.125 * (ranges >> (i * 6) & 0x3F);
                        RadarTargets[i].ClosingSpeed = threatLevel == ThreatLevel.None ? 0 : 3.04 * (speeds >> (i * 4) & 0x0F);
                    }
                    break;
                case DataPage.RadarTargetsB:
                    ranges = BitConverter.ToUInt32(dataPage, 3) & 0x00FFFFFF;
                    speeds = BitConverter.ToUInt16(dataPage, 6);
                    for (int i = 0; i < 4; i++)
                    {
                        var threatLevel = RadarTargets[i + 4].ThreatLevel = (ThreatLevel)((dataPage[1] >> (i * 2)) & 0x03);
                        RadarTargets[i + 4].ThreatSide = threatLevel == ThreatLevel.None ? ThreatSide.Behind : (ThreatSide)((dataPage[2] >> (i * 2)) & 0x03);
                        RadarTargets[i + 4].Range = threatLevel == ThreatLevel.None ? 0 : 3.125 * (ranges >> (i * 6) & 0x3F);
                        RadarTargets[i + 4].ClosingSpeed = threatLevel == ThreatLevel.None ? 0 : 3.04 * (speeds >> (i * 4) & 0x0F);
                    }
                    break;
                default:
                    CommonDataPages.ParseCommonDataPage(dataPage);
                    break;
            }
        }
        #endregion

        #region Custom Methods
        public enum Command
        {
            AbortShutdown,
            Shutdown
        }

        public async Task<MessagingReturnCode> Shutdown(Command command)
        {
            return await SendExtAcknowledgedMessage(new byte[8] { (byte)DataPage.DeviceCommand, (byte)command, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        }
        #endregion
    }
}
