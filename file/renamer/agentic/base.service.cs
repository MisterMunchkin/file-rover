using System.Text.Json;
using System.Threading.Tasks;
using file_rover.kernel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace file_rover.file.renamer.agentic;

//NOTE: Might not even be required.
public class FileRenamerAgenticService
{
    private readonly ChatHistoryAgentThread _agentThread = new();
    private readonly Kernel _kernel;
    private readonly ChatCompletionAgent _agent;

    public FileRenamerAgenticService()
    {
        _kernel = KernelBuilder.GetKernel();

        KernelPlugin fileRenamer = _kernel
            .CreatePluginFromType<FileRenamerPlugin>(FileRenamerPlugin.Name);
        _kernel.Plugins.Add(fileRenamer);

        _agent = new()
        {
            Kernel = _kernel,
            Arguments = new KernelArguments(new OllamaPromptExecutionSettings()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Required(),
                Temperature = 0.1f, // More deterministic since we want consistent renaming
                ServiceId = KernelBuilder.LLama3_1_8b
            }),
            Name = "FileRenamerAgent",
            Instructions = """
                You aren an agent that helps rename files withing a file system.

                You will be provided with the following information in the request:
                    - image_path: The path to the image file to be renamed.
                    - convention: The naming convention using metadata fields enclosed in double curly braces, e.g., {{field_name}}-{{another_field}}
                    - metadata_field_maps: A list of metadata fields to extract and use in the naming convention.

                Your task is to:
                    - Use the file extension to determine which plugin function to use to rename the file. (e.g., "jpg" -> "rename_image")
                    - Call the appropriate plugin function with the provided information.
                    - Return the result of the plugin function.
                    - If the plugin function throws an error, you will respond with the error message.

                Do not respond with a function call in JSON format. Always invoke the function and return its result.
            """
        };

        Console.WriteLine(":: FileRenamerAgent initialized ::");
    }

    public async Task TriggerRename(string filePath)
    {
        try
        {
            //TODO: Conventions and metadata fields should come from user config.
            var testConvention = "{{file_name}}-{{height}}x{{width}}-{{file_size}}";
            List<string> testMetadataFields = [
                "file_name",
                "height",
                "width",
                "file_size"
            ];

            var jsonRequest = JsonSerializer.Serialize(new RenameImageFileRequest
            {
                ImagePath = filePath,
                Convention = testConvention,
                MetadataFieldMaps = testMetadataFields
            }) ?? throw new JsonException("Failed to serialize rename request.");

            var message = new ChatMessageContent(AuthorRole.User, jsonRequest);

            DateTime now = DateTime.Now;
            KernelArguments arguments = new()
            {
                { "now", $"{now.ToShortDateString()} {now.ToShortTimeString()}"}
            };

            await foreach (ChatMessageContent response in _agent.InvokeAsync(message, _agentThread, options: new() { KernelArguments = arguments }))
            {
                Console.WriteLine($"AI response: {response.Content}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error triggering file rename: {ex.Message}");
        }
    }
}