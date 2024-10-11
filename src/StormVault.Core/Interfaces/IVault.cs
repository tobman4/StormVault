namespace StormVault.Core.Interfaces;

public interface IVault {
  // Save the vault to storage
  void SaveVault(string masterPW);
  void LoadVault(string masterPW);

  // Add a new password entry to the vault
  void AddPassword(string name, string plainPassword);

  // Retrieve a decrypted password entry by name
  string RetrievePassword(string name);

  // Remove a password entry by name
  void RemovePassword(string name);

  // List all password entries in the vault
  IEnumerable<string> ListPasswords();
}
