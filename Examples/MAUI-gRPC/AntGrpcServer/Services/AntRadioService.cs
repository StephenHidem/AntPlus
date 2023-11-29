using AntRadioGrpcService;
using Grpc.Core;
using SmallEarthTech.AntRadioInterface;
using System.Text;

namespace AntGrpcServer.Services
{
    public class AntRadioService(ILogger<AntRadioService> logger, IAntRadio antRadio) : AntRadio.AntRadioBase
    {
        private readonly ILogger<AntRadioService> _logger = logger;
        private readonly IAntRadio _antRadio = antRadio;

        public IAntChannel[]? AntChannels { get; private set; }

        public override Task<InitScanModeReply> InitializeContinuousScanMode(InitScanModeRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"{nameof(InitializeContinuousScanMode)}");
            AntChannels = _antRadio.InitializeContinuousScanMode();
            return Task.FromResult(new InitScanModeReply
            {
                NumChannels = AntChannels.Length
            });
        }

        public override Task<PropertiesReply> GetProperties(PropertiesRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"{nameof(GetProperties)}");
            SmallEarthTech.AntUsbStick.AntRadio usbAntRadio = (SmallEarthTech.AntUsbStick.AntRadio)_antRadio;
            AntResponse rsp = usbAntRadio.RequestMessageAndResponse(RequestMessageID.Version, 500);
            return Task.FromResult(new PropertiesReply
            {
                SerialString = usbAntRadio.GetSerialString(),
                HostVersion = Encoding.Default.GetString(rsp.Payload).TrimEnd('\0'),
                ProductDescription = usbAntRadio.GetProductDescription()
            });
        }
    }
}
