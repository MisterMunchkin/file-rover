namespace file_rover.kernel;

using Microsoft.SemanticKernel;

public class KernelBuilder
{
    private IKernelBuilder Builder { get; set; }

    public Kernel Kernel { get; set; }

    public KernelBuilder()
    {
        Builder = Kernel.CreateBuilder();

        Builder.AddOllamaChatCompletion(
            modelId: "llama3.1:8b",
            endpoint: new Uri("http://localhost:11434"),
            serviceId: "llama_agent"
        );
        Kernel = Builder.Build();
    }

    public Kernel GetKernel()
    {
        return Kernel.Clone();
    }
}