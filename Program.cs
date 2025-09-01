using Microsoft.Extensions.Configuration;
using file_rover.user.config;
using file_rover.file.system;
using file_rover.file.mutator.agentic;
using file_rover.file.renamer.agentic;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var userConfigService = new UserConfigService(configuration);
var fileSystemRunner = FileSystemRunner.Instance(userConfigService.WatchPath);


var fileMutatorAgenticService = new FileMutatorAgenticService(userConfigService.WatchPath);
fileMutatorAgenticService.Listen();

var fileRenamerAgenticService = new FileRenamerAgenticService();

while (true)
{
  Console.WriteLine("""
    Type 'toggle' to start/stop FileSystemRunner, 'exit' to quit:

    > type full path to a file to test FileRenamerAgenticService:
  """);
  var input = Console.ReadLine();

  if (input == "exit") break;

  if (input == "toggle")
  {
    if (fileSystemRunner.IsRunning)
    {
      fileSystemRunner.Stop();
      Console.WriteLine("FileSystemRunner stopped.");
    }
    else
    {
      fileSystemRunner.Start();
      Console.WriteLine("FileSystemRunner started.");
    }
    continue;
  }

  if (!string.IsNullOrEmpty(input) && File.Exists(input))
  {
    await fileRenamerAgenticService.TriggerRename(input);
    continue;
  }
}



fileSystemRunner.Dispose();
Console.WriteLine("Exiting...");

