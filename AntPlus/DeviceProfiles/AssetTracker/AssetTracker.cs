using AntRadioInterface;
using System.Linq;

namespace AntPlus.DeviceProfiles.AssetTracker
{
    /// <summary>
    /// This class serves as an asset tracker.
    /// </summary>
    public class AssetTracker : AntDevice
    {
        private bool idRequested;

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
        /// <summary>
        /// Gets the common data pages.
        /// </summary>
        public CommonDataPages CommonDataPages { get; private set; } = new CommonDataPages();

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetTracker"/> class.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        public AssetTracker(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
        }

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage"></param>
        public override void Parse(byte[] dataPage)
        {
            // check if asset list empty
            if (Assets.Count == 0 && idRequested == false)
            {
                // request ID pages
                RequestDataPage(DataPage.AssetId1, 255, 255, 4, CommandType.DataPageSet);
                idRequested = true;
            }

            Asset asset = Assets.FirstOrDefault(a => a.Index == (dataPage[1] & 0x1F));

            switch ((DataPage)dataPage[0])
            {
                case DataPage.AssetLocation1:
                    asset?.ParseLocation1(dataPage);
                    break;
                case DataPage.AssetLocation2:
                    asset?.ParseLocation2(dataPage);
                    break;
                case DataPage.NoAssets:
                    break;
                case DataPage.AssetId1:
                    if (asset == null)
                    {
                        asset = new Asset(dataPage);
                        Assets.Add(asset);
                    }
                    break;
                case DataPage.AssetId2:
                    asset?.ParseIdPage2(dataPage);
                    break;
                case DataPage.DisconnectCommand:
                    break;
                default:
                    CommonDataPages.ParseCommonDataPage(dataPage);
                    break;
            }
        }
    }
}
