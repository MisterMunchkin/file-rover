using Microsoft.SemanticKernel;
using Microsoft.Extensions.DependencyInjection;
using file_rover.service.model;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using file_rover.dto.file_organise;
using Microsoft.Extensions.Configuration;

var builder = Kernel.CreateBuilder();

// Register the custom TextEmbeddingService with a specific key
var textEmbeddingService = new TextEmbeddingService();
builder.Services.AddKeyedEmbeddingGenerator("nomic", textEmbeddingService);

// Register the custom chat completion service with a specific key
var chatCompletionService = new ChatCompletionService();
builder.Services.AddKeyedSingleton<IChatCompletionService>("gpt-oss", chatCompletionService);

var kernel = builder.Build();

var chat = kernel.GetRequiredService<IChatCompletionService>("gpt-oss");
var chatHistory = new ChatHistory();

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var organiseNewFileSystemMessage = $"{config["SystemMessagePath"]}/organise-new-file.mdc";
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


while (true)
{
    Console.Write("Enter file name: ");
    var input = Console.ReadLine();

    if (string.IsNullOrEmpty(input) || input.ToLower() == "exit") break;

    chatHistory.AddUserMessage($"A new file named `{input}` has been added to the folder. The current folder structure is as follows:\n\n```json\n{folderStructure}```\n\n");

    var response = await chat.GetChatMessageContentsAsync(chatHistory, executionSettings);
    Console.WriteLine($"AI Response: ${response[^1].Content}");
    Console.WriteLine("--------------------------------------------------");
}

Console.WriteLine("Exiting...");