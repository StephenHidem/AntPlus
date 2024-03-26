using AntRadioGrpcService;
using Cysharp.Net.Http;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    internal class AntGrpcClient
    {
        private readonly IPAddress _grpAddress = IPAddress.Parse("239.55.43.6");
        private const int _multicastPort = 55437;       // multicast port
        private IPAddress _serverIPAddress;

        private const int _gRPCPort = 5073;             // gRPC port
        private GrpcChannel _channel;
        private gRPCAntRadio.gRPCAntRadioClient _client;

        public async void SearchForService()
        {
            IPEndPoint multicastEndPoint = new(_grpAddress, _multicastPort);
            byte[] req = Encoding.ASCII.GetBytes("AntRadioService");
            using UdpClient udpClient = new(AddressFamily.InterNetwork);

            // send request for ANT radio server
            _ = udpClient.Send(req, req.Length, multicastEndPoint);
            UdpReceiveResult result = await udpClient.ReceiveAsync();
            _serverIPAddress = result.RemoteEndPoint.Address;

            // establish gRPC channel
            UriBuilder uriBuilder = new("http", _serverIPAddress.ToString(), _gRPCPort);
            _channel = GrpcChannel.ForAddress(uriBuilder.Uri, new GrpcChannelOptions
            {
                HttpHandler = new YetAnotherHttpHandler { Http2Only = true },
                DisposeHttpClient = true
            });
            _client = new gRPCAntRadio.gRPCAntRadioClient(_channel);

            // get ANT radio properties
            PropertiesReply reply = await _client.GetPropertiesAsync(new Empty());
            Debug.Log(reply);
        }
    }
}
