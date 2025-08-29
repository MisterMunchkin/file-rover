using Microsoft.Extensions.Configuration;
using file_rover.user.config;
using file_rover.file.system;
using file_rover.file.mutator.agentic;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var userConfigService = new UserConfigService(configuration);
var fileSystemRunner = FileSystemRunner.Instance(userConfigService.WatchPath);


var fileMutatorAgenticService = new FileMutatorAgenticOllamaService(userConfigService.WatchPath);
fileMutatorAgenticService.Listen();

while (true)
{
  Console.WriteLine("Type 'toggle' to start/stop FileSystemRunner, 'exit' to quit:"); ;
  var input = Console.ReadLine()?.Trim().ToLower();

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
  }

  Console.CancelKeyPress += (s, e) =>
  {
    Console.WriteLine("Cancellation requested, stopping...");
    fileSystemRunner.Dispose();
    Environment.Exit(0);
  };
}



fileSystemRunner.Dispose();
Console.WriteLine("Exiting...");

