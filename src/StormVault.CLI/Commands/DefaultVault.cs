using StormVault.CLI.Attributes;
using StormVault.Core.Interfaces;
using StormVault.Core.Vaults;
using System.ComponentModel.DataAnnotations;

namespace StormVault.CLI.Commands;

abstract class DefaultVault : AppCommand {

  [Required]
  [Option("--vault")]
  public FileInfo VaultFile { get; set; } = new(
    Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
      ".vault.json")
  );

  // [Required]
  [Option("-p")]
  public string? Password { get; set; } = null!;

  protected string GetPassword() {
    if(string.IsNullOrWhiteSpace(Password)) 
      return PromptForPassword("Password: ");
    else
      return Password;
  }

  protected IVault GetVault() {
    var vault = new JsonVault(VaultFile.FullName);
    
    if(VaultFile.Exists) {
      vault.LoadVault(GetPassword());
    }

    return vault;
  }

  public static string PromptForPassword(string prompt) {
    Console.Write(prompt);
    string password = string.Empty;

    // Read characters until Enter is pressed
    while (true) {
      var key = Console.ReadKey(intercept: true); // intercept: true to not show characters

      if (key.Key == ConsoleKey.Enter) {
        Console.WriteLine(); // Move to the next line after Enter
        break;
      } else if (key.Key == ConsoleKey.Backspace && password.Length > 0) {
        // Remove last character
        password = password[..^1];
        Console.Write("\b \b"); // Remove character from console display
      } else if (!char.IsControl(key.KeyChar)) {
        password += key.KeyChar;
        Console.Write("*"); // Show a placeholder character
      }
    }
    return password;
  }
}
