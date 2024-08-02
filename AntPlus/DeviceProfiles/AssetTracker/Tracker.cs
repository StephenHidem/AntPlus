using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker
{
    /// <summary>
    /// This class serves as an asset tracker.
    /// </summary>
    public partial class Tracker : AntDevice
    {
        /// <summary>
        /// The device type value transmitted in the channel ID.
        /// </summary>
        public const byte DeviceClass = 41;

        /// <inheritdoc/>
        public override int ChannelCount => 2048;

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

        /// <summary>
        /// The collection lock.
        /// </summary>
        /// <remarks>
        /// An application should use the collection lock to ensure thread safe access to the
        /// collection. For example, the code behind for a WPF window should include -
        /// <code>BindingOperations.EnableCollectionSynchronization(assetTracker.Assets, assetTracker.CollectionLock);</code>
        /// This ensures changes to the collection are thread safe and marshalled on the UI thread.
        /// </remarks>
        public object CollectionLock = new object();

        /// <summary>Gets the collection of assets being tracked.</summary>
        public ObservableCollection<Asset> Assets { get; } = new ObservableCollection<Asset>();
        /// <summary>Gets a value indicating whether this <see cref="Tracker" /> is disconnected.</summary>
        /// <value><c>true</c> if disconnected; otherwise, <c>false</c>.</value>
        [ObservableProperty]
        private bool disconnected;
        /// <summary>
        /// Gets the common data pages.
        /// </summary>
        public CommonDataPages CommonDataPages { get; private set; }
        /// <inheritdoc/>
        public override Stream DeviceImageStream => typeof(Tracker).Assembly.GetManifestResourceStream("SmallEarthTech.AntPlus.Images.AssetTracker.png");

        /// <summary>
        /// Initializes a new instance of the <see cref="Tracker"/> class.
        /// </summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, int?, byte?)"/>
        public Tracker(ChannelId channelId, IAntChannel antChannel, ILogger<Tracker> logger, int? timeout = default, byte? missedMessages = default)
            : base(channelId, antChannel, logger, timeout, missedMessages)
        {
            CommonDataPages = new CommonDataPages(logger);
        }

        /// <inheritdoc/>
        /// <remarks>The asset is removed from the <see cref="Assets"/> collection if the <see cref="Asset.AssetStatus.RemoveAsset"/> flag is set.</remarks>
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
                        lock (CollectionLock)
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
                        lock (CollectionLock)
                        {
                            Assets.Clear();
                        }
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
                lock (CollectionLock)
                {
                    Assets.Add(asset);
                }
                _ = RequestDataPage(DataPage.AssetId1, 500, 255, 255, 4, CommandType.DataPageSet);
            }
            return asset;
        }
    }
}
