using System.ComponentModel.DataAnnotations;
using StormVault.CLI.Attributes;

namespace StormVault.CLI.Commands;

[Command("add")]
class AddPassword : DefaultVault {
  
  [Required]
  [Option("--name")]
  public string Name { get; set; } = null!;

  [Required]
  [Option("--value")]
  public string Value { get; set; } = null!;

  /* public FileInfo SecretFile { get; set; } = new FileInfo( */
  /*   "./secret" */
  /* ); */


  private string GetSecret() {
    return Value;
  }

  protected override Task<int> InvokeAsync() {
    var vault = GetVault();

    vault.AddPassword(Name,GetSecret());
    vault.SaveVault(GetPassword());

    return Task.FromResult(0);
  }

}
