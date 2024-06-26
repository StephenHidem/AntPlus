﻿using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AntGrpcService.Services
{
    public class DiscoveryService(ILogger<DiscoveryService> logger) : BackgroundService
    {
        private readonly IPAddress grpAddress = IPAddress.Parse("239.55.43.6");    // IPv4 multicast group
        private const int multicastPort = 55437;        // multicast port
        private readonly ILogger<DiscoveryService> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // create IPv4 UDP client
            IPAddress? localIPAddress = Dns.GetHostAddresses(Dns.GetHostName()).Where(a => a.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault();
            if (localIPAddress == null)
            {
                _logger.LogError("Unable to get an IPv4 address for this host.");
                return;
            }
            IPEndPoint localIPEndPoint = new(localIPAddress, multicastPort);
            using UdpClient udpClient = new(localIPEndPoint);

            try
            {
                // join multicast group
                udpClient.JoinMulticastGroup(grpAddress);
                _logger.LogInformation("DiscoveryService running. Listening on {LocalIp}.", udpClient.Client.LocalEndPoint!.ToString());

                // Loop listening for service requests. UDP is not reliable and multiple clients may request access to service.
                while (!stoppingToken.IsCancellationRequested)
                {
                    // wait for request to connect to this server
                    var result = await udpClient.ReceiveAsync(stoppingToken);
                    _logger.LogInformation("DiscoveryService request: remote endpoint {RemoteEP}, message {Message}", result.RemoteEndPoint, Encoding.ASCII.GetString(result.Buffer));

                    // I could check the request for some sort of validation but I will ignore the message for now
                    // send response to requestor's endpoint with our service name
                    byte[] response = Encoding.ASCII.GetBytes("AntGrpcServer DiscoveryService response");
                    udpClient.Send(response, response.Length, result.RemoteEndPoint);
                }
            }
            catch (OperationCanceledException)
            {
                // When the stopping token is canceled, for example, a call made from services.msc,
                // we shouldn't exit with a non-zero exit code. In other words, this is expected...
                _logger.LogWarning("DiscoveryService cancelled.");
            }
            catch (Exception ex)
            {
                // Terminates this process and returns an exit code to the operating system.
                // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
                // performs one of two scenarios:
                // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
                // 2. When set to "StopHost": will cleanly stop the host, and log errors.
                //
                // In order for the Windows Service Management system to leverage configured
                // recovery options, we need to terminate the process with a non-zero exit code.
                _logger.LogError(ex, "{Message}", ex.Message);
                Environment.Exit(1);
            }
            finally
            {
                // drop multicast group when cancelled
                udpClient.DropMulticastGroup(grpAddress);
                udpClient.Close();
                _logger.LogWarning("DiscoveryService terminated.");
            }
        }
    }
}
