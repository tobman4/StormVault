using System.Security.Cryptography;
using StormVault.Core.Encryption;

public class VaultKeyManagerTests {
  private readonly VaultKeyManager _vaultKeyManager;

  public VaultKeyManagerTests() {
    _vaultKeyManager = new VaultKeyManager();
  }

  [Fact]
  public void GenerateVaultKey_ShouldReturnKeyWithCorrectLength() {
    // Act
    byte[] vaultKey = _vaultKeyManager.GenerateVaultKey();

    // Assert
    Assert.NotNull(vaultKey);
    Assert.Equal(32, vaultKey.Length); // AES-256 key length is 32 bytes
  }

  [Fact]
  public void EncryptVaultKey_ShouldEncryptKey() {
    // Arrange
    byte[] masterKey = GenerateRandomKey();
    byte[] vaultKey = _vaultKeyManager.GenerateVaultKey();

    // Act
    byte[] encryptedVaultKey = _vaultKeyManager.EncryptVaultKey(vaultKey, masterKey);

    // Assert
    Assert.NotNull(encryptedVaultKey);
    Assert.NotEqual(vaultKey, encryptedVaultKey); // The encrypted vault key should not be the same as the original
  }

  [Fact]
  public void DecryptVaultKey_ShouldReturnOriginalVaultKey() {
    // Arrange
    byte[] masterKey = GenerateRandomKey();
    byte[] vaultKey = _vaultKeyManager.GenerateVaultKey();
    byte[] encryptedVaultKey = _vaultKeyManager.EncryptVaultKey(vaultKey, masterKey);

    // Act
    byte[] decryptedVaultKey = _vaultKeyManager.DecryptVaultKey(encryptedVaultKey, masterKey);

    // Assert
    Assert.NotNull(decryptedVaultKey);
    Assert.Equal(vaultKey, decryptedVaultKey); // The decrypted key should match the original vault key
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

