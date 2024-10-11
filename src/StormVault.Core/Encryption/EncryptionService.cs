using System.Security.Cryptography;

namespace StormVault.Core.Encryption;

public class EncryptionService {
  // Encrypts the provided plainText using the provided key
  public byte[] Encrypt(string plainText, byte[] key) {
    using (Aes aes = Aes.Create()) {
      aes.Key = key;
      aes.GenerateIV(); // Generate a new IV for each encryption
      aes.Mode = CipherMode.CBC;
      aes.Padding = PaddingMode.PKCS7;

      using (var encryptor = aes.CreateEncryptor()) {
        using (var memoryStream = new MemoryStream()) {
          // Write the IV to the memory stream first
          memoryStream.Write(aes.IV, 0, aes.IV.Length);

          using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) {
            using (var writer = new StreamWriter(cryptoStream)) {
              writer.Write(plainText);
            }
          }

          return memoryStream.ToArray();
        }
      }
    }
  }

  // Decrypts the provided cipherText using the provided key
  public string Decrypt(byte[] cipherTextWithIV, byte[] key) {
    using (Aes aes = Aes.Create()) {
      aes.Key = key;
      aes.Mode = CipherMode.CBC;
      aes.Padding = PaddingMode.PKCS7;

      // Extract the IV from the cipherText
      byte[] iv = new byte[aes.BlockSize / 8];
      byte[] actualCipherText = new byte[cipherTextWithIV.Length - iv.Length];

      Buffer.BlockCopy(cipherTextWithIV, 0, iv, 0, iv.Length);
      Buffer.BlockCopy(cipherTextWithIV, iv.Length, actualCipherText, 0, actualCipherText.Length);

      aes.IV = iv;

      using (var decryptor = aes.CreateDecryptor()) {
        using (var memoryStream = new MemoryStream(actualCipherText)) {
          using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)) {
            using (var reader = new StreamReader(cryptoStream)) {
              return reader.ReadToEnd();
            }
          }
        }
      }
    }
  }
}

