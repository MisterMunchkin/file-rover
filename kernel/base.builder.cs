namespace file_rover.kernel;

using Microsoft.SemanticKernel;

public static class KernelBuilder
{
    private static IKernelBuilder? Builder { get; set; }

    public static Kernel? Kernel { get; set; }

    public static readonly string LLama3_1_8b = "llama3.1:8b";
    public static readonly string LLama3_2_3b = "llama3.2:3b";


    public static void Build()
    {
        Builder = Kernel.CreateBuilder();

        Builder.AddOllamaChatCompletion(
            modelId: LLama3_1_8b,
            endpoint: new Uri("http://localhost:11434"),
            serviceId: LLama3_1_8b
        );
        Builder.AddOllamaChatCompletion(
            modelId: LLama3_2_3b,
            endpoint: new Uri("http://localhost:11434"),
            serviceId: LLama3_2_3b
        );

        Kernel = Builder.Build();
    }

    public static Kernel GetKernel()
    {
        if (Kernel == null)
            Build();
        
        if (Kernel == null)
            throw new InvalidOperationException("Kernel failed to build.");

        return Kernel.Clone();
    }
}