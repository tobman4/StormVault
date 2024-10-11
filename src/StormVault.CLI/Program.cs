// See https://aka.ms/new-console-template for more information
using System.CommandLine;
using System.CommandLine.Parsing;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using StormVault.CLI.Commands;
using System.Security.Cryptography;

var root = new RootCommand();
var builder = new CommandLineBuilder(root);
builder.UseDefaults();

root.Add(new AddPassword().Build());
root.Add(new ListCommand().Build());
root.Add(new GetCommand().Build());
root.Add(new DeleteCommand().Build());

builder.AddMiddleware(async(context,next) => {
  AppCommand.Context = context;
  await next(context);
}, MiddlewareOrder.Default);

builder.AddMiddleware(async(context,next) => {
  bool didShitBed = true;

  try {
    await next(context);
    didShitBed = false;
  } catch(CryptographicException) {
    Console.WriteLine("Error while encrypting or decrypting");

  } catch(Exception err) {
    Console.WriteLine($"Error: {err.Message}");
  }

  if(didShitBed) context.ExitCode = 1;
});

var parser = builder.Build();
await parser.InvokeAsync(args);
