using System.Security.Cryptography;
using StormVault.Core.Encryption;


public class EncryptionServiceTests {
  private readonly EncryptionService _encryptionService;

  public EncryptionServiceTests() {
    _encryptionService = new EncryptionService();
  }

  [Fact]
  public void EncryptAndDecrypt_ShouldReturnOriginalText() {
    // Arrange
    string originalText = "MySecretPassword123!";
    byte[] key = GenerateRandomKey();

    // Act
    byte[] encryptedText = _encryptionService.Encrypt(originalText, key);
    string decryptedText = _encryptionService.Decrypt(encryptedText, key);

    // Assert
    Assert.Equal(originalText, decryptedText); // The decrypted text should match the original
  }

  [Fact]
  public void Encrypt_ShouldProduceDifferentResultsForSameInput() {
    // Arrange
    string plainText = "SamePassword";
    byte[] key = GenerateRandomKey();

    // Act
    byte[] encryptedText1 = _encryptionService.Encrypt(plainText, key);
    byte[] encryptedText2 = _encryptionService.Encrypt(plainText, key);

    // Assert
    Assert.NotEqual(encryptedText1, encryptedText2); // The encrypted results should be different
  }

  // Helper method to generate a random AES key (32 bytes for AES-256)
  private byte[] GenerateRandomKey() {
    byte[] key = new byte[32];
    using (var rng = RandomNumberGenerator.Create()) {
      rng.GetBytes(key);
    }
    return key;
  }
}

