﻿using AntRadioInterface;
using System.Linq;

namespace AntPlus.DeviceProfiles.AssetTracker
{
    /// <summary>
    /// Tracks assets.
    /// 
    /// © 2022 Stephen Hidem.
    /// </summary>
    public class AssetTracker : AntDevice
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
            AssetLocation1 = 0x01,
            AssetLocation2 = 0x02,
            NoAssets = 0x03,
            AssetId1 = 0x10,
            AssetId2 = 0x11,
            DisconnectCommand = 0x20,
        }

        /// <summary>Gets the assets.</summary>
        /// <value>The assets being tracked.</value>
        public AssetCollection Assets { get; } = new AssetCollection();
        public CommonDataPages CommonDataPages { get; private set; } = new CommonDataPages();

        public AssetTracker(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
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
                    CommonDataPages.ParseCommonDataPage(dataPage);
                    break;
            }
        }
    }
}
