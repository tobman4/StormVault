namespace StormVault.CLI.Attributes;

class ArgumentAttribute : Attribute {
  public readonly string Name;
  public string Description { get; init; } = "";

  public ArgumentAttribute(string name) {
    Name = name;
  }
}
