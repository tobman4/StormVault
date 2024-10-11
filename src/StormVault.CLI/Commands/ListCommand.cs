using StormVault.CLI.Attributes;

namespace StormVault.CLI.Commands;

[Command("list")]
class ListCommand : DefaultVault {
  protected override Task<int> InvokeAsync() {
    var vault = GetVault();

    foreach(var pw in vault.ListPasswords())
      Console.WriteLine(pw);

    return Task.FromResult(0);
  }
}
