using AntRadioInterface;
using System;
using System.Linq;

namespace AntPlus.DeviceProfiles.AssetTracker
{
    public class AssetTracker : AntDevice
    {
        /// The asset tracker device class ID.
        public const byte DeviceClass = 41;

        /// <summary>
        /// Main data pages.
        /// </summary>
        public enum DataPage
        {
            AssetLocation1 = 0x01,
            AssetLocation2 = 0x02,
            NoAssets = 0x03,
            AssetId1 = 0x10,
            AssetId2 = 0x11,
            DisconnectCommand = 0x20,
        }

        public AssetCollection Assets { get; } = new AssetCollection();

        public AssetTracker(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
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

        private bool idRequested;
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
                    break;
            }
        }
    }
}
