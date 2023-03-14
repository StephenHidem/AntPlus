namespace SmallEarthTech.AntRadioInterface
{
    /// <summary>Encryption non-volatile memory operations.</summary>
    public enum EncryptionNVMOp : byte
    {
        /// <summary>The load key from NVM</summary>
        LoadKeyFromNvm = 0x00,
        /// <summary>The store key to NVM</summary>
        StoreKeyToNvm = 0x01,
    }

    /// <summary>Encryption information.</summary>
    public enum EncryptionInfo : byte
    {
        /// <summary>The encryption identifier</summary>
        EncryptionId = 0x00,
        /// <summary>The user information string</summary>
        UserInfoString = 0x01,
        /// <summary>The random number seed</summary>
        RandomNumberSeed = 0x02,
    };

    /// <summary>This interface defines ANT encryption commands.</summary>
    public interface ICrypto
    {
        /// <summary>Crypto key NVM operation.</summary>
        /// <param name="memoryOperation">The memory operation.</param>
        /// <param name="nonVolatileKeyIndex">Index of the non volatile key.</param>
        /// <param name="operationData">The operation data.</param>
        void CryptoKeyNVMOp(EncryptionNVMOp memoryOperation, byte nonVolatileKeyIndex, byte[] operationData);
        /// <summary>Crypto key NVM operation.</summary>
        /// <param name="memoryOperation">The memory operation.</param>
        /// <param name="nonVolatileKeyIndex">Index of the non volatile key.</param>
        /// <param name="operationData">The operation data.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool CryptoKeyNVMOp(EncryptionNVMOp memoryOperation, byte nonVolatileKeyIndex, byte[] operationData, uint responseWaitTime);
        /// <summary>Loads the crypto key from NVM.</summary>
        /// <param name="nonVolatileKeyIndex">Index of the non volatile key.</param>
        /// <param name="volatileKeyIndex">Index of the volatile key.</param>
        void LoadCryptoKeyFromNVM(byte nonVolatileKeyIndex, byte volatileKeyIndex);
        /// <summary>Loads the crypto key from NVM.</summary>
        /// <param name="nonVolatileKeyIndex">Index of the non volatile key.</param>
        /// <param name="volatileKeyIndex">Index of the volatile key.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool LoadCryptoKeyFromNVM(byte nonVolatileKeyIndex, byte volatileKeyIndex, uint responseWaitTime);
        /// <summary>Sets the crypto identifier.</summary>
        /// <param name="encryptionID">The encryption identifier.</param>
        void SetCryptoID(byte[] encryptionID);
        /// <summary>Sets the crypto identifier.</summary>
        /// <param name="encryptionID">The encryption identifier.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool SetCryptoID(byte[] encryptionID, uint responseWaitTime);
        /// <summary>Sets the crypto information.</summary>
        /// <param name="encryptionParameter">The encryption parameter.</param>
        /// <param name="parameterData">The parameter data.</param>
        void SetCryptoInfo(EncryptionInfo encryptionParameter, byte[] parameterData);
        /// <summary>Sets the crypto information.</summary>
        /// <param name="encryptionParameter">The encryption parameter.</param>
        /// <param name="parameterData">The parameter data.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool SetCryptoInfo(EncryptionInfo encryptionParameter, byte[] parameterData, uint responseWaitTime);
        /// <summary>Sets the crypto key.</summary>
        /// <param name="volatileKeyIndex">Index of the volatile key.</param>
        /// <param name="encryptionKey">The encryption key.</param>
        void SetCryptoKey(byte volatileKeyIndex, byte[] encryptionKey);
        /// <summary>Sets the crypto key.</summary>
        /// <param name="volatileKeyIndex">Index of the volatile key.</param>
        /// <param name="encryptionKey">The encryption key.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool SetCryptoKey(byte volatileKeyIndex, byte[] encryptionKey, uint responseWaitTime);
        /// <summary>Sets the crypto RNG seed.</summary>
        /// <param name="cryptoRNGSeed">The crypto RNG seed.</param>
        void SetCryptoRNGSeed(byte[] cryptoRNGSeed);
        /// <summary>Sets the crypto RNG seed.</summary>
        /// <param name="cryptoRNGSeed">The crypto RNG seed.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool SetCryptoRNGSeed(byte[] cryptoRNGSeed, uint responseWaitTime);
        /// <summary>Sets the crypto user information.</summary>
        /// <param name="userInfoString">The user information string.</param>
        void SetCryptoUserInfo(byte[] userInfoString);
        /// <summary>Sets the crypto user information.</summary>
        /// <param name="userInfoString">The user information string.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool SetCryptoUserInfo(byte[] userInfoString, uint responseWaitTime);
        /// <summary>Stores the crypto key to NVM.</summary>
        /// <param name="nonVolatileKeyIndex">Index of the non volatile key.</param>
        /// <param name="encryptionKey">The encryption key.</param>
        void StoreCryptoKeyToNVM(byte nonVolatileKeyIndex, byte[] encryptionKey);
        /// <summary>Stores the crypto key to NVM.</summary>
        /// <param name="nonVolatileKeyIndex">Index of the non volatile key.</param>
        /// <param name="encryptionKey">The encryption key.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool StoreCryptoKeyToNVM(byte nonVolatileKeyIndex, byte[] encryptionKey, uint responseWaitTime);
    }
}
