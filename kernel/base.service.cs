namespace file_rover.kernel;

using file_rover.file.mutator;
using file_rover.kernel.lm_studio;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

public class KernelService
{
    private IKernelBuilder Builder { get; set; }

    public Kernel Kernel { get; set; }

    public KernelService()
    {
        Builder = Kernel.CreateBuilder();

        // Builder.Services.AddKeyedEmbeddingGenerator("nomic", new TextEmbeddingService());
        Builder.Services.AddKeyedSingleton<IChatCompletionService>("gpt-oss", new ChatCompletionService());
        // Builder.AddOllamaChatCompletion(
        //     modelId: "llama3.2:8b",
        //     endpoint: new Uri("http://localhost:11434")
        // );
        Builder.Plugins.AddFromType<FileMutatorPlugin>("file_mutator");

        Kernel = Builder.Build();
    }

    public IChatCompletionService GetChatService()
    {
        return Kernel.GetRequiredService<IChatCompletionService>("gpt-oss");
    }
}