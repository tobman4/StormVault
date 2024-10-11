using System.Security.Cryptography;

namespace StormVault.Core.Encryption;

public class VaultKeyManager {
  // Generates a new vault AES key
  public byte[] GenerateVaultKey() {
    using (Aes aes = Aes.Create()) {
      aes.KeySize = 256;
      aes.GenerateKey();
      return aes.Key;
    }
  }

  // Encrypts the vault key using the master key
  public byte[] EncryptVaultKey(byte[] vaultKey, byte[] masterKey) {
    using (Aes aes = Aes.Create()) {
      aes.Key = masterKey;
      aes.GenerateIV(); // Ensure a unique IV is used for each encryption
      aes.Mode = CipherMode.CBC;
      aes.Padding = PaddingMode.PKCS7;

      using (var encryptor = aes.CreateEncryptor()) {
        byte[] encryptedVaultKey = PerformCryptography(vaultKey, encryptor);

        // Combine IV and encrypted data for easier storage
        byte[] result = new byte[aes.IV.Length + encryptedVaultKey.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(encryptedVaultKey, 0, result, aes.IV.Length, encryptedVaultKey.Length);

        return result;
      }
    }
  }

  // Decrypts the vault key using the master key
  public byte[] DecryptVaultKey(byte[] encryptedVaultKeyWithIV, byte[] masterKey) {
    using (Aes aes = Aes.Create()) {
      aes.Key = masterKey;
      aes.Mode = CipherMode.CBC;
      aes.Padding = PaddingMode.PKCS7;

      // Extract IV from the start of the encrypted data
      byte[] iv = new byte[aes.BlockSize / 8];
      byte[] encryptedVaultKey = new byte[encryptedVaultKeyWithIV.Length - iv.Length];

      Buffer.BlockCopy(encryptedVaultKeyWithIV, 0, iv, 0, iv.Length);
      Buffer.BlockCopy(encryptedVaultKeyWithIV, iv.Length, encryptedVaultKey, 0, encryptedVaultKey.Length);

      aes.IV = iv;

      using (var decryptor = aes.CreateDecryptor()) {
        return PerformCryptography(encryptedVaultKey, decryptor);
      }
    }
  }

  // Utility method to perform encryption and decryption
  private byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform) {
    using (var memoryStream = new System.IO.MemoryStream()) {
      using (var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write)) {
        cryptoStream.Write(data, 0, data.Length);
      }
      return memoryStream.ToArray();
    }
  }
}

