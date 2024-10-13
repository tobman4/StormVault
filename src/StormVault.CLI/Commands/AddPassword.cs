using System.ComponentModel.DataAnnotations;
using System.Text;
using StormVault.CLI.Attributes;

namespace StormVault.CLI.Commands;

[Command("add")]
class AddPassword : DefaultVault {
  
  [Argument("name")]
  public string Name { get; set; } = null!;

  /* [Required] */
  /* [Option("--value")] */
  /* public string Value { get; set; } = null!; */

  [Option("-n", Description = "Dont try to trim white space of end ot the secret")]
  public bool NoTrim { get; set; }

  [Required]
  [Option("-f", Description = "File that contains secret")]
  public FileInfo SecretFile { get; set; } = new FileInfo(
    "./secret"
  );

  private string GetSecret() {
    if(!SecretFile.Exists)
      return string.Empty;

    var reader = SecretFile.OpenRead();
    
    var bytes = new byte[reader.Length];
    reader.Read(bytes);

    var str = Encoding.UTF8.GetString(bytes); 

    if(NoTrim)
      return str;
    
    return str.Trim();
  }

  protected override Task<int> InvokeAsync() {
    var vault = GetVault();

    vault.AddPassword(Name,GetSecret());
    vault.SaveVault(GetPassword());

    return Task.FromResult(0);
  }

}
