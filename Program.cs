using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using file_rover.dto.file_organise;
using Microsoft.Extensions.Configuration;
using file_rover.service;
using file_rover.dto.user;
using System.Text.Json;

var kernelService = new KernelService();

var chat = kernelService.GetChatService();
var chatHistory = new ChatHistory();

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var organiseNewFileSystemMessage = Path.Combine(config["SystemMessagePath"]!, "organise-new-file.mdc");
string systemMessageContent = File.ReadAllText(organiseNewFileSystemMessage);
chatHistory.AddSystemMessage(systemMessageContent);
Console.WriteLine("System message loaded from file." + systemMessageContent);

var executionSettings = new AzureOpenAIPromptExecutionSettings {
    ResponseFormat = typeof(FileOrganiseResponseDto)
};
string folderStructure = @"{
  ""name"": ""Downloads"",
  ""subFolders"": [
    {
      ""name"": ""NBI"",
      ""subFolders"": [],
      ""files"": [
        ""att.VWlaQ4iii-MFIBjZ0eViepRvvm4hGkGpN9Ylcy-iD3E.JPG"",
        ""att._OirQH6_aAlSeBE0HTofllFpwXoKDWCiX3Rv-f1JLLQ.JPG""
      ]
    },
    {
      ""name"": ""Robins Payslip"",
      ""subFolders"": [],
      ""files"": [
        ""PaySlip(1).pdf"",
        ""PaySlip.pdf"",
        ""Robin Tubungbanua Payslip Mar 9 2025.pdf""
      ]
    },
    {
      ""name"": ""earnings"",
      ""subFolders"": [],
      ""files"": [
        ""Earnings Apr 2024 to Mar 2025.csv"",
        ""Earnings Report Mar 22 2025.pdf"",
        ""Equifax Tenant Report 2025.pdf""
      ]
    },
    {
      ""name"": ""Perry House"",
      ""subFolders"": [],
      ""files"": []
    }
  ],
  ""files"": [
    "".DS_Store"",
    "".localized"",
    ""ArcRecoveryPhrase.png"",
    ""Azure Template.zip"",
    ""Extreme Ownership Jocko Willink.epub"",
    ""Fri, Jul 11, 2025 9:18 PM - bouldernana export.json"",
    ""IMG_9814.MOV"",
    ""IMG_9815.MOV"",
    ""Perry House Parcel Locker Instructions.pdf"",
    ""Residential Tenancy Agreement Apr 4 2025 Bowen Hills.pdf"",
    ""The 5 Love Languages.epub"",
    ""Tue, Jul 8, 2025 7:16 PM - bouldernana export.json""
  ]
}";

string configPath = config["UserConfigPath"]!;

string userConfigJson = File.ReadAllText(configPath);

UserConfig userConfig = JsonSerializer.Deserialize<UserConfig>(userConfigJson);

// var fileSystemRunner = FileSystemRunner.Instance(userConfig.FileSystemWatcher.WatchPath);
var fileSystemRunner = FileSystemRunner.Instance(userConfig.FileSystemWatcher.WatchPath);

fileSystemRunner.FileReady += async (sender, e) =>
{
  _ = Task.Run(async () => {
    try
    {
      Console.WriteLine($"File ready event received for file: {e}");
      var fileName = Path.GetFileName(e);
      chatHistory.AddUserMessage($"A new file named `{fileName}` has been added to the folder. file_path: {e}. The current folder structure is as follows:\n\n```json\n{folderStructure}```\n\n");

      // Get AI response and run Kernel Function for moving file.

      var response = await chat.GetChatMessageContentsAsync(chatHistory, executionSettings);
      Console.WriteLine($"AI Response: ${response[^1].Content}");
      Console.WriteLine("--------------------------------------------------");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error processing file ready event: {ex.Message}");
    }
  });
 
};

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

