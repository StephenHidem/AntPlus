using AntCryptoGrpcService;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SmallEarthTech.AntRadioInterface;

namespace AntGrpcService.Services
{
    public class AntCryptoService(IAntRadio antRadio) : gRPCAntCrypto.gRPCAntCryptoBase
    {
        private readonly ICrypto _crypto = (ICrypto)antRadio;

        public override Task<BoolValue> CryptoKeyNVMOp(CryptoKeyNVMOpRequest request, ServerCallContext context)
        {
            bool result = _crypto.CryptoKeyNVMOp((SmallEarthTech.AntRadioInterface.EncryptionNVMOp)request.MemoryOperation, (byte)request.NonVolatileKeyIndex, request.OperationData.ToByteArray(), request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        override public Task<BoolValue> LoadCryptoKeyFromNVM(LoadCryptoKeyFromNVMRequest request, ServerCallContext context)
        {
            bool result = _crypto.LoadCryptoKeyFromNVM((byte)request.NonVolatileKeyIndex, (byte)request.VolatileKeyIndex, request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        override public Task<BoolValue> SetCryptoID(SetCryptoIDRequest request, ServerCallContext context)
        {
            bool result = _crypto.SetCryptoID(request.EncryptionID.ToByteArray(), request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        override public Task<BoolValue> SetCryptoInfo(SetCryptoInfoRequest request, ServerCallContext context)
        {
            bool result = _crypto.SetCryptoInfo((SmallEarthTech.AntRadioInterface.EncryptionInfo)request.EncryptionParameter, request.ParameterData.ToByteArray(), request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        override public Task<BoolValue> SetCryptoKey(SetCryptoKeyRequest request, ServerCallContext context)
        {
            bool result = _crypto.SetCryptoKey((byte)request.VolatileKeyIndex, request.EncryptionKey.ToByteArray(), request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        override public Task<BoolValue> SetCryptoRNGSeed(SetCryptoRNGSeedRequest request, ServerCallContext context)
        {
            bool result = _crypto.SetCryptoRNGSeed(request.CryptoRNGSeed.ToByteArray(), request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        override public Task<BoolValue> SetCryptoUserInfo(SetCryptoUserInfoRequest request, ServerCallContext context)
        {
            bool result = _crypto.SetCryptoUserInfo(request.UserInfoString.ToByteArray(), request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        override public Task<BoolValue> StoreCryptoKeyToNVM(StoreCryptoKeyToNVMRequest request, ServerCallContext context)
        {
            bool result = _crypto.StoreCryptoKeyToNVM((byte)request.NonVolatileKeyIndex, request.EncryptionKey.ToByteArray(), request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }
    }
}
