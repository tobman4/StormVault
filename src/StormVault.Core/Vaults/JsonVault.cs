using System.Security.Cryptography;
using System.Text.Json;
using StormVault.Core.Encryption;
using StormVault.Core.Interfaces;
using StormVault.Core.Models;

namespace StormVault.Core.Vaults;

public class JsonVault : IVault {
  private readonly string _filePath = null!;
  private readonly EncryptionService _encryptionService;
  private byte[] _vaultKey = null!;
  private byte[]? _salt;
  private readonly List<PasswordEntry> _passwordEntries;

  public JsonVault(string path) {
    _encryptionService = new EncryptionService();
    _passwordEntries = new List<PasswordEntry>();
    _filePath = path;
    GenerateVaultKey();
  }

  // Generates a new vault key (AES key for encrypting passwords)
  private void GenerateVaultKey() {
    using (Aes aes = Aes.Create()) {
      aes.KeySize = 256;
      aes.GenerateKey();
      _vaultKey = aes.Key;
    }
  }

  // Add a new password to the vault and encrypt it using the vault key
  public void AddPassword(string name, string plainPassword) {
    if (_vaultKey == null) throw new Exception("Vault key not loaded.");

    // Encrypt the password using the vault key
    byte[] encryptedPassword = _encryptionService.Encrypt(plainPassword, _vaultKey);

    // Create a new password entry
    var entry = new PasswordEntry(name, Convert.ToBase64String(encryptedPassword));
    _passwordEntries.Add(entry);
  }

  // Retrieve and decrypt a password from the vault
  public string RetrievePassword(string name) {
    if (_vaultKey == null) throw new Exception("Vault key not loaded.");

    var entry = _passwordEntries.FirstOrDefault(e => e.Name == name);
    if (entry == null) {
      throw new Exception("Password entry not found.");
    }

    // Decrypt the password using the vault key
    byte[] encryptedPasswordBytes = Convert.FromBase64String(entry.EncryptedPassword);
    return _encryptionService.Decrypt(encryptedPasswordBytes, _vaultKey);
  }

  // Remove a password entry by name
  public void RemovePassword(string name) {
    var entry = _passwordEntries.FirstOrDefault(e => e.Name == name);

    if (entry != null) {
      _passwordEntries.Remove(entry);
    } else {
      throw new Exception("Password entry not found.");
    }
  }

  // List all password entry names in the vault
  public IEnumerable<string> ListPasswords() {
    return _passwordEntries.Select(e => e.Name);
  }

  // Save the vault to a JSON file
  public void SaveVault(string masterPW) {
    if (_vaultKey == null) throw new Exception("Vault key not loaded.");

    var masterKey = GetKey(masterPW);

    // Encrypt the vault key using the master key
    byte[] encryptedVaultKey = _encryptionService.Encrypt(Convert.ToBase64String(_vaultKey), masterKey);

    // Convert the password entries and the encrypted vault key to a JSON string
    var vaultData = new {
      EncryptedVaultKey = Convert.ToBase64String(encryptedVaultKey),
      Salt = Convert.ToBase64String(_salt),
      PasswordEntries = _passwordEntries
    };

    string json = JsonSerializer.Serialize(vaultData, new JsonSerializerOptions {
      WriteIndented = true
    });

    // Write the JSON string to the file
    File.WriteAllText(_filePath, json);
  }

  // Load the vault from a JSON file
  public void LoadVault(string masterPw) {
    if (!File.Exists(_filePath)) {
      throw new Exception("Vault file not found.");
    }

    // Read the JSON data from the file
    string json = File.ReadAllText(_filePath);


    // Deserialize the JSON data
    var vaultData = JsonSerializer.Deserialize<VaultData>(json);
    if (vaultData == null) throw new Exception("Failed to load vault data.");

    // Get encrypted key and salt
    byte[] encryptedVaultKey = Convert.FromBase64String(vaultData.EncryptedVaultKey);
    _salt = Convert.FromBase64String(vaultData.Salt);

    var masterKey = GetKey(masterPw);

    // Decrypt the vault key using the master key
    string decryptedVaultKeyString = _encryptionService.Decrypt(encryptedVaultKey, masterKey);
    _vaultKey = Convert.FromBase64String(decryptedVaultKeyString);

    // Load the password entries
    _passwordEntries.Clear();
    if (vaultData.PasswordEntries != null) {
      _passwordEntries.AddRange(vaultData.PasswordEntries);
    }
  }
  
  private byte[] GetKey(string pw) {
    var iterations = 100_000;
    if(_salt is null) {
      _salt = new byte[16];
      new Random().NextBytes(_salt);
    }

    using(var keyGen = new Rfc2898DeriveBytes(pw,_salt,iterations, HashAlgorithmName.SHA256)) {
      return keyGen.GetBytes(32);
    }
  }

  // Helper class to represent the structure of the vault data in JSON
  private class VaultData {
    public string EncryptedVaultKey { get; set; } = null!;
    public string Salt { get; set; } = null!;
    public List<PasswordEntry> PasswordEntries { get; set; } = new List<PasswordEntry>();
  }
}

