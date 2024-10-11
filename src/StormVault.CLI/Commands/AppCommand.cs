using System.CommandLine;
using System.CommandLine.Invocation;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using StormVault.CLI.Attributes;

namespace StormVault.CLI.Commands;

abstract class AppCommand {

  public int Test;
 
  internal static InvocationContext Context = null!;

  private readonly Dictionary<PropertyInfo,Option> _options = new();

  private void LoadArguments() {
    var classType = this.GetType();
    foreach(var prop in classType.GetProperties()) {
      if(prop.GetCustomAttribute<OptionAttribute>() is OptionAttribute attr)
        AddProp(prop,attr);
      
      //TODO: Argument

    }
  }


  private void AddProp(PropertyInfo prop, OptionAttribute attr) {
    if(_options.ContainsKey(prop))
      return;

    var optionType = typeof(Option<>).MakeGenericType(prop.PropertyType);

    var optObj = Activator.CreateInstance(optionType, [
      attr.Name, attr.Description
    ]);

    if(optObj is not Option opt) 
      throw new Exception($"Faild to create option {attr.Name}");

    if(prop.GetValue(this) is object def)
      opt.SetDefaultValue(def);
    opt.IsRequired = prop.GetCustomAttribute<RequiredAttribute>() is not null;
    _options.Add(prop,opt);
  }

  protected abstract Task<int> InvokeAsync();
  private Task<int> _run() {

    foreach(var kv in _options) {
      var value = Context.ParseResult.GetValueForOption(kv.Value);
      kv.Key.SetValue(this,value);
    }

    return InvokeAsync();
  }

  public Command Build() {
    var type = this.GetType();
    var command = new Command("whitespace");
    command.SetHandler(_run);
    
    //Use class name if command have no CommandAttribute
    if(type.GetCustomAttribute<CommandAttribute>() is CommandAttribute attr) {
      command.Name = attr.Name;
      command.Description = attr.Description;
    } else {
      command.Name = type.Name;
    }


    LoadArguments();
    foreach(var kv in _options)
      command.Add(kv.Value);

    return command;
  }

}
