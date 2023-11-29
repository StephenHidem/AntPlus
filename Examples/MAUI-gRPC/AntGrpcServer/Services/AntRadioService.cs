using AntRadioGrpcService;
using Grpc.Core;
using SmallEarthTech.AntRadioInterface;

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
    }
}
