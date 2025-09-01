using file_rover.file.system;
using file_rover.kernel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace file_rover.file.mutator.agentic;

public class FileMutatorAgenticService
{
  private readonly ChatHistoryAgentThread _agentThread = new();
  private readonly Kernel _kernel;
  private readonly string _watchPath;

  private readonly ChatCompletionAgent _agent;

  public FileMutatorAgenticService(
    string watchPath
  )
  {
    _watchPath = watchPath;
    _kernel = KernelBuilder
      .GetKernel();

    KernelPlugin fileMutator = _kernel.CreatePluginFromType<FileMutatorPlugin>(FileMutatorPlugin.Name);
    _kernel.Plugins.Add(fileMutator);
    var validFoldersList = GetValidFolders();

    _agent = new()
    {
      Kernel = _kernel,
      Arguments = new KernelArguments(new OllamaPromptExecutionSettings()
      {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Required(),
        Temperature = 0.6f, // Add some randomness to encourage exploration
        ServiceId = KernelBuilder.LLama3_2_3b
      })
      {
        {"watch_path", _watchPath},
        {"folders_list", validFoldersList }
      },
      Name = "FileMutatorAgent",
      Instructions = $"""
        You are an agent that helps organise a file within a file system.

        - You may ONLY move files into the following allowed folders: {validFoldersList}.
        - The root folder is: {_watchPath}.
        - Never use any folder outside of {_watchPath}. Do NOT invent paths like "Documents" or "Downloads".
        - If no existing folder matches, move the file into "Uncategorised".
        - If you think a new folder might be needed, ask the user to confirm before creating it.

        Do not respond with a function call in JSON format. Always invoke the function and return its result.
      """,
    };

    Console.WriteLine(":: FileMutatorAgent initialized ::");
    Console.WriteLine($":: Watch Path: {_watchPath} ::");
  }


  public void Listen()
  {
    var fileSystemRunner = FileSystemRunner.Instance(_watchPath);
    fileSystemRunner.FileReady += OnFileReady;
  }

  private void OnFileReady(object? sender, string e)
  {
    _ = Task.Run(async () =>
    {
      try
      {
        Console.WriteLine($"File ready event received for file: {e}");
        var fileName = Path.GetFileName(e);
        var message = new ChatMessageContent(AuthorRole.User, $"""
          A new file named `{fileName}` has been added to the folder. 
          file_path: {e}.

          Where would you move the file `{fileName}`?
        """);

        DateTime now = DateTime.Now;
        KernelArguments arguments = new()
        {
            { "now", $"{now.ToShortDateString()} {now.ToShortTimeString()}" }
        };

        await foreach (ChatMessageContent response in _agent.InvokeAsync(message, _agentThread, options: new() { KernelArguments = arguments }))
        {
          // Display response.
          Console.WriteLine($"AI Response: {response.Content}");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error processing file ready event: {ex.Message}");
      }
    });
  }

  private string GetValidFolders() {
    var validFolders = Directory
    .GetDirectories(_watchPath, "*", SearchOption.AllDirectories)
    .Select(Path.GetFileName)
    .ToList();

    var foldersList = string.Join(", ", validFolders);

    return foldersList;
  }
}