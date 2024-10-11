namespace StormVault.CLI.Attributes;

class OptionAttribute : Attribute {

  public readonly string Name;
  public string Description { get; init; } = "";

  public OptionAttribute(string name) {
    Name = name;
  }
}
