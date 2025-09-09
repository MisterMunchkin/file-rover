namespace file_rover.kernel;

using Microsoft.SemanticKernel;
using LLamaSharp.SemanticKernel;
using Microsoft.Extensions.DependencyInjection;
 
public static class KernelEmbeddedBuilder
{
    private static IKernelBuilder? Builder { get; set; }

    public static Kernel? Kernel { get; set; }



    public static void Build()
    {
        throw new NotImplementedException("De-prioed in favor of the renamer");
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