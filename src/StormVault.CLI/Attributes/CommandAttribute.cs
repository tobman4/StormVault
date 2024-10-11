namespace StormVault.CLI.Attributes;

class CommandAttribute : Attribute {

  public readonly string Name;
  public string Description { get; init; } = "";

  public CommandAttribute(string name) {
    Name = name;
  }
}
