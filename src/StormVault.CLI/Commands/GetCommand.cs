using StormVault.CLI.Attributes;
using System.ComponentModel.DataAnnotations;

namespace StormVault.CLI.Commands;

[Command("get")]
class GetCommand : DefaultVault {

  [Required]
  [Option("--name", Description = "Name of secret to read")]
  public string Name { get; set; } = null!;

  protected override Task<int> InvokeAsync() {
    var vault = GetVault();
    
    var secret = vault.RetrievePassword(Name);
    Console.WriteLine(secret);

    return Task.FromResult(0);
  }
}
