using System.Collections.Concurrent;

namespace AntPlus
{
    //public enum EventMsgId
    //{
    //    RxSearchTimeout = 0x01,
    //    RxFailed = 0x02,
    //    TxSuccess = 0x03,
    //    TransferRxFailed = 0x04,
    //    TransferTxCompleted = 0x05,
    //    TransferTxFailed = 0x06,
    //    ChannelClosed = 0x07,
    //    RxFailGoToSearch = 0x08,
    //    ChannelCollision = 0x09,
    //    TransferTxStart = 0x0A,
    //    TransferNextDataBlock = 0x11,
    //    SerialQueueOverflow = 0x34,
    //    QueueOverflow = 0x35
    //}

    //public enum ResponseMsgId
    //{
    //    NoError = 0x00,
    //    ChannelInWrongState = 0x15,
    //    ChannelNotOpened = 0x16,
    //    ChannelIdNotSet = 0x18,
    //    CloseAllChannels = 0x19,
    //    TransferInProgress = 0x1F,
    //    TransferSeqNumberError = 0x20,
    //    TransferInError = 0x21,
    //    MsgSizeExceedsLimit = 0x27,
    //    InvalidMsg = 0x28,
    //    InvalidNetworkNumber = 0x29,
    //    InvalidListId = 0x30,
    //    InvalidScanTxChannels = 0x31,
    //    InvalidParameter = 0x33,
    //    EncryptNegotiationSucces = 0x38,
    //    EncryptNegotiationFail = 0x39,
    //    NvmFullError = 0x40,
    //    NvmWriteError = 0x41,
    //    UsbStringWriteFail = 0x70,
    //    MsgSerialErrorId = 0xAE
    //}

    internal class AntRadio
    {
        private enum ConfigurationMsgId
        {
            UnassignChannel = 0x41,
            AssignChannel = 0x42,
            ChannelId = 0x51,
            ChannelPeriod = 0x43,
            SearchTimeout = 0x44,
            ChannelRfFrequency = 0x45,
            SetNetworkKey = 0x46,
            TransmitPower = 0x47,
            SearchWaveform = 0x49,
            SetChannelTxPower = 0x60,
            LowPrioritySearchTimeout = 0x63,
            SerialNumberSetChannelId = 0x65,
            EnableExtRxMsg = 0x66,
            EnableLed = 0x68,
            CrystalEnable = 0x6D,
            LibConfig = 0x6E,
            FrequencyAgility = 0x70,
            ProximitySearch = 0x71,
            CfgEventBuffer = 0x74,
            ChannelSearchPriority = 0x75,
            Set128BitNetworkKey = 0x76,
            HighDutySearch = 0x77,
            CfgAdvanceBurst = 0x78,
            CfgEventFilter = 0x79,
            CfgSelectiveDataUpdates = 0x7A,
            SetSelectiveDataUpdateMask = 0x7B,
            CfgUserNVM = 0x7C,
            EnableSingleChannelEncryption = 0x7D,
            SetEncryptionKey = 0x7E,
            SetEncryptionInfo = 0x7F,
            ChannelSearchSharing = 0x81,
            LoadStoreEncryptionKey = 0x83,
            SetUsbDescriptorString = 0xC7
        }

        private enum Notifications
        {
            Startup = 0x6F,
            SerialError = 0xAE
        }

        private const byte ChannelMsg = 0x40;

        private enum RequestedResponseMsg
        {
            ChannelStatus = 0x52,
            ChannelId = 0x51,
            ANTVersion = 0x3E,
            Capabilities = 0x54,
            SerialNumber = 0x61,
            EventBufferCfg = 0x74,
            AdvancedBurst = 0x78,
            EventFilter = 0x79,
            SelectiveUpdateMask = 0x7B,
            UserNVM = 0x7C,
            EncryptionModeParms = 0x7D
        }

        private enum TestMode
        {
            CWInit = 0x53,
            CWTest = 0x48
        }

        private ConcurrentQueue<AntDevice> antDevices = new ConcurrentQueue<AntDevice>();
        //private ConcurrentQueue<byte[]> radioMessages = new ConcurrentQueue<byte[]>();
        private AntDevice[] ants = new AntDevice[8];

        private void ChannelMessageHandler(byte[] message)
        {
            if (message[1] == 1)
            {
                // channel event
                ants[message[0]].ChannelEventHandler((EventMsgId)message[2]);
            }
            else
            {
                // channel response
                ants[message[0]].ChannelResponseHandler(message[1], (ResponseMsgId)message[2]);
            }
        }

    }
}
