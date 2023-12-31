using ANT_Managed_Library;
using SmallEarthTech.AntRadioInterface;

namespace SmallEarthTech.AntUsbStick
{
    public partial class AntRadio : ICrypto
    {
        /// <inheritdoc/>
        public void CryptoKeyNVMOp(EncryptionNVMOp memoryOperation, byte nonVolatileKeyIndex, byte[] operationData)
        {
            _antDevice.cryptoKeyNVMOp((ANT_ReferenceLibrary.EncryptionNVMOp)memoryOperation, nonVolatileKeyIndex, operationData);
        }

        /// <inheritdoc/>
        public bool CryptoKeyNVMOp(EncryptionNVMOp memoryOperation, byte nonVolatileKeyIndex, byte[] operationData, uint responseWaitTime)
        {
            return _antDevice.cryptoKeyNVMOp((ANT_ReferenceLibrary.EncryptionNVMOp)memoryOperation, nonVolatileKeyIndex, operationData, responseWaitTime);
        }

        /// <inheritdoc/>
        public void LoadCryptoKeyFromNVM(byte nonVolatileKeyIndex, byte volatileKeyIndex) => _antDevice.loadCryptoKeyFromNVM(nonVolatileKeyIndex, volatileKeyIndex);

        /// <inheritdoc/>
        public bool LoadCryptoKeyFromNVM(byte nonVolatileKeyIndex, byte volatileKeyIndex, uint responseWaitTime) => _antDevice.loadCryptoKeyFromNVM(nonVolatileKeyIndex, volatileKeyIndex, responseWaitTime);

        /// <inheritdoc/>
        public void SetCryptoID(byte[] encryptionID) => _antDevice.setCryptoID(encryptionID);

        /// <inheritdoc/>
        public bool SetCryptoID(byte[] encryptionID, uint responseWaitTime) => _antDevice.setCryptoID(encryptionID, responseWaitTime);

        /// <inheritdoc/>
        public void SetCryptoInfo(EncryptionInfo encryptionParameter, byte[] parameterData) => _antDevice.setCryptoInfo((ANT_ReferenceLibrary.EncryptionInfo)encryptionParameter, parameterData);

        /// <inheritdoc/>
        public bool SetCryptoInfo(EncryptionInfo encryptionParameter, byte[] parameterData, uint responseWaitTime) => _antDevice.setCryptoInfo((ANT_ReferenceLibrary.EncryptionInfo)encryptionParameter, parameterData, responseWaitTime);

        /// <inheritdoc/>
        public void SetCryptoKey(byte volatileKeyIndex, byte[] encryptionKey) => _antDevice.setCryptoKey(volatileKeyIndex, encryptionKey);

        /// <inheritdoc/>
        public bool SetCryptoKey(byte volatileKeyIndex, byte[] encryptionKey, uint responseWaitTime) => _antDevice.setCryptoKey(volatileKeyIndex, encryptionKey, responseWaitTime);

        /// <inheritdoc/>
        public void SetCryptoRNGSeed(byte[] cryptoRNGSeed) => _antDevice.setCryptoRNGSeed(cryptoRNGSeed);

        /// <inheritdoc/>
        public bool SetCryptoRNGSeed(byte[] cryptoRNGSeed, uint responseWaitTime) => _antDevice.setCryptoRNGSeed(cryptoRNGSeed, responseWaitTime);

        /// <inheritdoc/>
        public void SetCryptoUserInfo(byte[] userInfoString) => _antDevice.setCryptoUserInfo(userInfoString);

        /// <inheritdoc/>
        public bool SetCryptoUserInfo(byte[] userInfoString, uint responseWaitTime) => _antDevice.setCryptoUserInfo(userInfoString, responseWaitTime);

        /// <inheritdoc/>
        public void StoreCryptoKeyToNVM(byte nonVolatileKeyIndex, byte[] encryptionKey) => _antDevice.storeCryptoKeyToNVM(nonVolatileKeyIndex, encryptionKey);

        /// <inheritdoc/>
        public bool StoreCryptoKeyToNVM(byte nonVolatileKeyIndex, byte[] encryptionKey, uint responseWaitTime) => _antDevice.storeCryptoKeyToNVM(nonVolatileKeyIndex, encryptionKey, responseWaitTime);
    }
}
