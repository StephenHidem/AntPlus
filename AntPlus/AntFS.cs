using System;
using System.Linq;

namespace AntFileShare
{
    public enum BeaconChannelPeriod
    {
        _0_5Hz,
        _1Hz,
        _2Hz,
        _4Hz,
        _8Hz,
        MatchEstablishedChannelPeriod = 7
    }

    public enum ClientDeviceState
    {
        Link,
        Authentication,
        Transport,
        Busy
    }

    public enum AuthenticationType
    {
        PassThru,
        Reserved,
        PairingOnly,
        PassKeyAndPairing
    }

    public enum Command
    {
        Link = 0x02,
        Disconnect = 0x03,
        Authenticate = 0x04,
        Ping = 0x05,
        DownloadRequest = 0x09,
        UploadRequest = 0x0A,
        EraseRequest = 0x0B,
        UploadData = 0x0C
    }

    public enum CommandResponse
    {
        Authenticate = 0x84,
        DownloadRequest = 0x89,
        UploadRequest = 0x8A,
        EraseRequest = 0x8B,
        UploadData = 0x8C
    }

    public enum AuthenticationResponse
    {
        None,
        Accept,
        Reject
    }

    public enum DownloadRequestResponse
    {
        Ok,
        NoData,
        NotDownloadable,
        NotReady,
        Invalid,
        BadCrc
    }

    public enum UploadRequestResponse
    {
        Ok,
        FileDoesNotExist,
        FileNotWriteable,
        NotEnoughSpace,
        Invalid,
        NotReady
    }

    public enum UploadDataResponse
    {
        Successful,
        Failed
    }

    public enum EraseResponse
    {
        Successful,
        Failed,
        NotReady
    }

    public class AntFS
    {
        // Client Beacon
        public bool DataAvailable { get; private set; }
        public bool UploadEnabled { get; private set; }
        public bool PairingEnabled { get; private set; }
        public BeaconChannelPeriod ChannelPeriod { get; private set; }
        public ClientDeviceState DeviceState { get; private set; }
        public AuthenticationType Authentication { get; private set; }
        public uint DeviceDescriptor { get; private set; }
        public uint HostSerialNumber { get; private set; }
        public uint ClientSerialNumber { get; private set; }
        public byte[] AuthenicationString { get; private set; }
        public AuthenticationResponse AuthenticationResponse { get; private set; }

        public DownloadRequestResponse DownloadResponse { get; private set; }
        public int TotalRemaingDataLength { get; private set; }
        public uint DataOffset { get; private set; }
        public uint FileSize { get; private set; }
        public byte[] DownloadData { get; private set; }

        public UploadRequestResponse UploadResponse { get; private set; }
        public uint LastDataOffset { get; private set; }
        public uint MaxFileSize { get; private set; }
        public uint MaxBlockSize { get; private set; }

        public UploadDataResponse UploadDataResponse { get; private set; }

        public EraseResponse EraseResponse { get; private set; }

        public void ParseClientBeacon(byte[] payload)
        {
            // parse status byte 1
            ChannelPeriod = (BeaconChannelPeriod)(payload[1] & 0x07);
            DataAvailable = (payload[1] & 0x20) == 0x20;
            UploadEnabled = (payload[1] & 0x10) == 0x10;
            PairingEnabled = (payload[1] & 0x08) == 0x08;

            // parse status byte 2
            DeviceState = (ClientDeviceState)(payload[2] & 0x0F);

            Authentication = (AuthenticationType)payload[3];

            switch (DeviceState)
            {
                case ClientDeviceState.Link:
                    DeviceDescriptor = BitConverter.ToUInt32(payload, 4);
                    break;
                case ClientDeviceState.Authentication:
                case ClientDeviceState.Transport:
                    HostSerialNumber = BitConverter.ToUInt32(payload, 4);
                    break;
                case ClientDeviceState.Busy:
                    break;
                default:
                    break;
            }
        }

        public void ParseCommandResponse(byte[] response)
        {
            ushort crc;

            switch ((CommandResponse)response[1])
            {
                case CommandResponse.Authenticate:
                    AuthenticationResponse = (AuthenticationResponse)response[2];
                    ClientSerialNumber = BitConverter.ToUInt32(response, 4);
                    if (response[3] != 0)
                    {
                        // authentication string exists, save it
                        AuthenicationString = response.Skip(8).Take(response[3]).ToArray();
                    }
                    break;
                case CommandResponse.DownloadRequest:
                    DownloadResponse = (DownloadRequestResponse)response[2];
                    TotalRemaingDataLength = BitConverter.ToInt32(response, 4);
                    DataOffset = BitConverter.ToUInt32(response, 8);
                    FileSize = BitConverter.ToUInt32(response, 12);
                    DownloadData = response.Skip(16).Take(response.Length - 8).ToArray();
                    crc = BitConverter.ToUInt16(response, response.Length - 2);
                    break;
                case CommandResponse.UploadRequest:
                    UploadResponse = (UploadRequestResponse)response[2];
                    LastDataOffset = BitConverter.ToUInt32(response, 4);
                    MaxFileSize = BitConverter.ToUInt32(response, 8);
                    MaxBlockSize = BitConverter.ToUInt32(response, 12);
                    crc = (ushort)BitConverter.ToUInt16(response, 22);
                    break;
                case CommandResponse.EraseRequest:
                    EraseResponse = (EraseResponse)response[2];
                    break;
                case CommandResponse.UploadData:
                    UploadDataResponse = (UploadDataResponse)response[2];
                    break;
                default:
                    break;
            }
        }
    }
}
