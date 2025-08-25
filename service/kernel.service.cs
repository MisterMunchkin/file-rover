using file_rover.plugins;
using file_rover.service.model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace file_rover.service;

public class KernelService
{
    private IKernelBuilder Builder { get; set; }

    public Kernel Kernel { get; set; }

    public KernelService()
    {
        Builder = Kernel.CreateBuilder();

        Builder.Services.AddKeyedEmbeddingGenerator("nomic", new TextEmbeddingService());
        Builder.Services.AddKeyedSingleton<IChatCompletionService>("gpt-oss", new ChatCompletionService());
        Builder.Plugins.AddFromType<FileMutatorPlugin>("file_mutator");

        Kernel = Builder.Build();
    }

    public IChatCompletionService GetChatService()
    {
        return Kernel.GetRequiredService<IChatCompletionService>("gpt-oss");
    }
}