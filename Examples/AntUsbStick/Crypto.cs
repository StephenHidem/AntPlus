using ANT_Managed_Library;
using SmallEarthTech.AntRadioInterface;

namespace SmallEarthTech.AntUsbStick
{
    public partial class AntRadio : ICrypto
    {
        /// <inheritdoc/>
        public bool CryptoKeyNVMOp(EncryptionNVMOp memoryOperation, byte nonVolatileKeyIndex, byte[] operationData, uint responseWaitTime = 0)
        {
            return _antDevice.cryptoKeyNVMOp((ANT_ReferenceLibrary.EncryptionNVMOp)memoryOperation, nonVolatileKeyIndex, operationData, responseWaitTime);
        }

        /// <inheritdoc/>
        public bool LoadCryptoKeyFromNVM(byte nonVolatileKeyIndex, byte volatileKeyIndex, uint responseWaitTime = 0) => _antDevice.loadCryptoKeyFromNVM(nonVolatileKeyIndex, volatileKeyIndex, responseWaitTime);

        /// <inheritdoc/>
        public bool SetCryptoID(byte[] encryptionID, uint responseWaitTime = 0) => _antDevice.setCryptoID(encryptionID, responseWaitTime);

        /// <inheritdoc/>
        public bool SetCryptoInfo(EncryptionInfo encryptionParameter, byte[] parameterData, uint responseWaitTime = 0) => _antDevice.setCryptoInfo((ANT_ReferenceLibrary.EncryptionInfo)encryptionParameter, parameterData, responseWaitTime);

        /// <inheritdoc/>
        public bool SetCryptoKey(byte volatileKeyIndex, byte[] encryptionKey, uint responseWaitTime = 0) => _antDevice.setCryptoKey(volatileKeyIndex, encryptionKey, responseWaitTime);

        /// <inheritdoc/>
        public bool SetCryptoRNGSeed(byte[] cryptoRNGSeed, uint responseWaitTime = 0) => _antDevice.setCryptoRNGSeed(cryptoRNGSeed, responseWaitTime);

        /// <inheritdoc/>
        public bool SetCryptoUserInfo(byte[] userInfoString, uint responseWaitTime = 0) => _antDevice.setCryptoUserInfo(userInfoString, responseWaitTime);

        /// <inheritdoc/>
        public bool StoreCryptoKeyToNVM(byte nonVolatileKeyIndex, byte[] encryptionKey, uint responseWaitTime = 0) => _antDevice.storeCryptoKeyToNVM(nonVolatileKeyIndex, encryptionKey, responseWaitTime);
    }
}
