using ANT_Managed_Library;
using SmallEarthTech.AntRadioInterface;

namespace SmallEarthTech.AntUsbStick
{
    public partial class AntRadio : ICrypto
    {
        /// <inheritdoc/>
        public void CryptoKeyNVMOp(EncryptionNVMOp memoryOperation, byte nonVolatileKeyIndex, byte[] operationData)
        {
            antDevice.cryptoKeyNVMOp((ANT_ReferenceLibrary.EncryptionNVMOp)memoryOperation, nonVolatileKeyIndex, operationData);
        }

        /// <inheritdoc/>
        public bool CryptoKeyNVMOp(EncryptionNVMOp memoryOperation, byte nonVolatileKeyIndex, byte[] operationData, uint responseWaitTime)
        {
            return antDevice.cryptoKeyNVMOp((ANT_ReferenceLibrary.EncryptionNVMOp)memoryOperation, nonVolatileKeyIndex, operationData, responseWaitTime);
        }

        /// <inheritdoc/>
        public void LoadCryptoKeyFromNVM(byte nonVolatileKeyIndex, byte volatileKeyIndex) => antDevice.loadCryptoKeyFromNVM(nonVolatileKeyIndex, volatileKeyIndex);

        /// <inheritdoc/>
        public bool LoadCryptoKeyFromNVM(byte nonVolatileKeyIndex, byte volatileKeyIndex, uint responseWaitTime) => antDevice.loadCryptoKeyFromNVM(nonVolatileKeyIndex, volatileKeyIndex, responseWaitTime);

        /// <inheritdoc/>
        public void SetCryptoID(byte[] encryptionID) => antDevice.setCryptoID(encryptionID);

        /// <inheritdoc/>
        public bool SetCryptoID(byte[] encryptionID, uint responseWaitTime) => antDevice.setCryptoID(encryptionID, responseWaitTime);

        /// <inheritdoc/>
        public void SetCryptoInfo(EncryptionInfo encryptionParameter, byte[] parameterData) => antDevice.setCryptoInfo((ANT_ReferenceLibrary.EncryptionInfo)encryptionParameter, parameterData);

        /// <inheritdoc/>
        public bool SetCryptoInfo(EncryptionInfo encryptionParameter, byte[] parameterData, uint responseWaitTime) => antDevice.setCryptoInfo((ANT_ReferenceLibrary.EncryptionInfo)encryptionParameter, parameterData, responseWaitTime);

        /// <inheritdoc/>
        public void SetCryptoKey(byte volatileKeyIndex, byte[] encryptionKey) => antDevice.setCryptoKey(volatileKeyIndex, encryptionKey);

        /// <inheritdoc/>
        public bool SetCryptoKey(byte volatileKeyIndex, byte[] encryptionKey, uint responseWaitTime) => antDevice.setCryptoKey(volatileKeyIndex, encryptionKey, responseWaitTime);

        /// <inheritdoc/>
        public void SetCryptoRNGSeed(byte[] cryptoRNGSeed) => antDevice.setCryptoRNGSeed(cryptoRNGSeed);

        /// <inheritdoc/>
        public bool SetCryptoRNGSeed(byte[] cryptoRNGSeed, uint responseWaitTime) => antDevice.setCryptoRNGSeed(cryptoRNGSeed, responseWaitTime);

        /// <inheritdoc/>
        public void SetCryptoUserInfo(byte[] userInfoString) => antDevice.setCryptoUserInfo(userInfoString);

        /// <inheritdoc/>
        public bool SetCryptoUserInfo(byte[] userInfoString, uint responseWaitTime) => antDevice.setCryptoUserInfo(userInfoString, responseWaitTime);

        /// <inheritdoc/>
        public void StoreCryptoKeyToNVM(byte nonVolatileKeyIndex, byte[] encryptionKey) => antDevice.storeCryptoKeyToNVM(nonVolatileKeyIndex, encryptionKey);

        /// <inheritdoc/>
        public bool StoreCryptoKeyToNVM(byte nonVolatileKeyIndex, byte[] encryptionKey, uint responseWaitTime) => antDevice.storeCryptoKeyToNVM(nonVolatileKeyIndex, encryptionKey, responseWaitTime);
    }
}
