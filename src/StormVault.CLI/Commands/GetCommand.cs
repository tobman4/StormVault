using StormVault.CLI.Attributes;

namespace StormVault.CLI.Commands;

[Command("get")]
class GetCommand : DefaultVault {

  [Argument("name", Description = "Name of secret to read")]
  public string Name { get; set; } = null!;

  protected override Task<int> InvokeAsync() {
    var vault = GetVault();
    
    var secret = vault.RetrievePassword(Name);
    Console.WriteLine(secret);

    return Task.FromResult(0);
  }
}
