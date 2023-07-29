using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System.IO;
using System.Linq;

namespace SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker
{
    /// <summary>
    /// This class serves as an asset tracker.
    /// </summary>
    public class Tracker : AntDevice
    {
        /// <summary>
        /// The asset tracker device class ID.
        /// </summary>
        public const byte DeviceClass = 41;

        /// <summary>
        /// Main data pages.
        /// </summary>
        public enum DataPage
        {
            /// <summary>Asset location page 1</summary>
            AssetLocation1 = 0x01,
            /// <summary>Asset location page 2</summary>
            AssetLocation2 = 0x02,
            /// <summary>No assets</summary>
            NoAssets = 0x03,
            /// <summary>Asset identifier page 1</summary>
            AssetId1 = 0x10,
            /// <summary>Asset identifier page 2</summary>
            AssetId2 = 0x11,
            /// <summary>Disconnect command</summary>
            DisconnectCommand = 0x20,
        }

        /// <summary>Gets the collection of assets being tracked.</summary>
        public AssetCollection Assets { get; } = new AssetCollection();
        /// <summary>Gets a value indicating whether this <see cref="Tracker" /> is disconnected.</summary>
        /// <value><c>true</c> if disconnected; otherwise, <c>false</c>.</value>
        public bool Disconnected { get; private set; }
        /// <summary>
        /// Gets the common data pages.
        /// </summary>
        public CommonDataPages CommonDataPages { get; private set; }
        /// <inheritdoc/>
        public override Stream DeviceImageStream => typeof(Tracker).Assembly.GetManifestResourceStream("SmallEarthTech.AntPlus.Images.AssetTracker.png");

        /// <summary>
        /// Initializes a new instance of the <see cref="Tracker"/> class. The default timeout is 500ms.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        /// <param name="logger">Logger to use.</param>
        /// <param name="timeout">Time in milliseconds before firing <see cref="AntDevice.DeviceWentOffline"/>.</param>
        public Tracker(ChannelId channelId, IAntChannel antChannel, ILogger<Tracker> logger, int timeout = 500) : base(channelId, antChannel, logger, timeout)
        {
            CommonDataPages = new CommonDataPages(logger);
        }

        /// <inheritdoc/>
        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);
            switch ((DataPage)dataPage[0])
            {
                case DataPage.AssetLocation1:
                    Asset asset = GetAsset(dataPage);
                    asset.ParseLocation1(dataPage);
                    if (asset.Status.HasFlag(Asset.AssetStatus.RemoveAsset))
                    {
                        lock (Assets.CollectionLock)
                        {
                            Assets.Remove(asset);
                        }
                    }
                    break;
                case DataPage.AssetLocation2:
                    GetAsset(dataPage).ParseLocation2(dataPage);
                    break;
                case DataPage.NoAssets:
                    if (Assets.Count > 0)
                    {
                        Assets.Clear();
                    }
                    break;
                case DataPage.AssetId1:
                    GetAsset(dataPage).ParseIdPage1(dataPage);
                    break;
                case DataPage.AssetId2:
                    GetAsset(dataPage).ParseIdPage2(dataPage);
                    break;
                case DataPage.DisconnectCommand:
                    Disconnected = true;
                    RaisePropertyChange(nameof(Disconnected));
                    break;
                default:
                    CommonDataPages.ParseCommonDataPage(dataPage);
                    break;
            }
        }

        internal Asset GetAsset(byte[] data)
        {
            Asset asset = Assets.FirstOrDefault(a => a.Index == (data[1] & 0x1F));
            if (asset == null)
            {
                asset = new Asset(data);
                Assets.Add(asset);
                _ = RequestDataPage(DataPage.AssetId1, 500, 255, 255, 4, CommandType.DataPageSet);
            }
            return asset;
        }
    }
}
