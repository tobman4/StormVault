using System.Security.Cryptography;
using StormVault.Core.Vaults;

public class JsonVaultTests : IDisposable {
  private readonly JsonVault _vault;
  private readonly string _masterPW = "SuperSecret69";
  private readonly string _tempFilePath;

  public JsonVaultTests() {
    _tempFilePath = Path.GetTempFileName(); // Create a temporary file
    _vault = new JsonVault(_tempFilePath);
  }

  [Fact]
  public void SaveAndLoadVault_ShouldPersistPasswords() {
    // Arrange
    string passwordName = "example.com";
    string password = "MySecurePassword123";

    // Act
    _vault.AddPassword(passwordName, password);
    _vault.SaveVault(_masterPW);

    // Load the vault back
    var newVault = new JsonVault(_tempFilePath);
    newVault.LoadVault(_masterPW);
    string retrievedPassword = newVault.RetrievePassword(passwordName);

    // Assert
    Assert.Equal(password, retrievedPassword); // Ensure the password is correctly retrieved after saving and loading
  }

  // Dispose pattern to clean up the temporary file
  public void Dispose() {
    if (File.Exists(_tempFilePath)) {
      File.Delete(_tempFilePath); // Ensure the temporary file is deleted after the test
    }
  }
}
