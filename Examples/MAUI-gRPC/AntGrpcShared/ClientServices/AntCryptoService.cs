using AntCryptoGrpcService;
using SmallEarthTech.AntRadioInterface;

namespace AntGrpcShared.ClientServices
{
    public partial class AntRadioService : ICrypto
    {
        private gRPCAntCrypto.gRPCAntCryptoClient? _crypto;

        /// <inheritdoc/>
        public bool CryptoKeyNVMOp(SmallEarthTech.AntRadioInterface.EncryptionNVMOp memoryOperation, byte nonVolatileKeyIndex, byte[] operationData, uint responseWaitTime = 0)
        {
            return _crypto!.CryptoKeyNVMOp(new CryptoKeyNVMOpRequest { MemoryOperation = (AntCryptoGrpcService.EncryptionNVMOp)memoryOperation, NonVolatileKeyIndex = nonVolatileKeyIndex, OperationData = Google.Protobuf.ByteString.CopyFrom(operationData), ResponseWaitTime = responseWaitTime }).Value;
        }

        /// <inheritdoc/>
        public bool LoadCryptoKeyFromNVM(byte nonVolatileKeyIndex, byte volatileKeyIndex, uint responseWaitTime = 0)
        {
            return _crypto!.LoadCryptoKeyFromNVM(new LoadCryptoKeyFromNVMRequest { NonVolatileKeyIndex = nonVolatileKeyIndex, VolatileKeyIndex = volatileKeyIndex, ResponseWaitTime = responseWaitTime }).Value;
        }

        /// <inheritdoc/>
        public bool SetCryptoID(byte[] encryptionID, uint responseWaitTime = 0)
        {
            return _crypto!.SetCryptoID(new SetCryptoIDRequest { EncryptionID = Google.Protobuf.ByteString.CopyFrom(encryptionID), ResponseWaitTime = responseWaitTime }).Value;
        }

        /// <inheritdoc/>
        public bool SetCryptoInfo(SmallEarthTech.AntRadioInterface.EncryptionInfo encryptionParameter, byte[] parameterData, uint responseWaitTime = 0)
        {
            return _crypto!.SetCryptoInfo(new SetCryptoInfoRequest { EncryptionParameter = (AntCryptoGrpcService.EncryptionInfo)encryptionParameter, ParameterData = Google.Protobuf.ByteString.CopyFrom(parameterData), ResponseWaitTime = responseWaitTime }).Value;
        }

        /// <inheritdoc/>
        public bool SetCryptoKey(byte volatileKeyIndex, byte[] encryptionKey, uint responseWaitTime = 0)
        {
            return _crypto!.SetCryptoKey(new SetCryptoKeyRequest { VolatileKeyIndex = volatileKeyIndex, EncryptionKey = Google.Protobuf.ByteString.CopyFrom(encryptionKey), ResponseWaitTime = responseWaitTime }).Value;
        }

        /// <inheritdoc/>
        public bool SetCryptoRNGSeed(byte[] cryptoRNGSeed, uint responseWaitTime = 0)
        {
            return _crypto!.SetCryptoRNGSeed(new SetCryptoRNGSeedRequest { CryptoRNGSeed = Google.Protobuf.ByteString.CopyFrom(cryptoRNGSeed), ResponseWaitTime = responseWaitTime }).Value;
        }

        /// <inheritdoc/>
        public bool SetCryptoUserInfo(byte[] userInfoString, uint responseWaitTime = 0)
        {
            return _crypto!.SetCryptoUserInfo(new SetCryptoUserInfoRequest { UserInfoString = Google.Protobuf.ByteString.CopyFrom(userInfoString), ResponseWaitTime = responseWaitTime }).Value;
        }

        /// <inheritdoc/>
        public bool StoreCryptoKeyToNVM(byte nonVolatileKeyIndex, byte[] encryptionKey, uint responseWaitTime = 0)
        {
            return _crypto!.StoreCryptoKeyToNVM(new StoreCryptoKeyToNVMRequest { NonVolatileKeyIndex = nonVolatileKeyIndex, EncryptionKey = Google.Protobuf.ByteString.CopyFrom(encryptionKey), ResponseWaitTime = responseWaitTime }).Value;
        }
    }
}
