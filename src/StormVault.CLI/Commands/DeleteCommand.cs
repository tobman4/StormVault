using StormVault.CLI.Attributes;

namespace StormVault.CLI.Commands;

[Command("rm")]
class DeleteCommand : DefaultVault {

  [Argument("name")]
  public string Name { get; set; } = null!;

  protected override Task<int> InvokeAsync() {
    var vault = GetVault();

    vault.RemovePassword(Name);
    vault.SaveVault(GetPassword());

    return Task.FromResult(0);
  }
}
