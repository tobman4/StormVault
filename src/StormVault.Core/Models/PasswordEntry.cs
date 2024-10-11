namespace StormVault.Core.Models;

public class PasswordEntry {
    public string Name { get; set; } = null!;
    public DateTime CreationTime { get; set; } = DateTime.UtcNow;
    public string EncryptedPassword { get; set; } = null!;

    public PasswordEntry(string name, string encryptedPassword) {
        Name = name;
        EncryptedPassword = encryptedPassword;
    }

    public override string ToString() {
        return $"Name: {Name}, Created: {CreationTime}, Encrypted Password: {EncryptedPassword}";
    }
}
