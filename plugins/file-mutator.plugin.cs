
using System.ComponentModel;
using file_rover.dto.file;
using file_rover.dto.user;
using Microsoft.SemanticKernel;

namespace file_rover.plugins;

public class FileMutatorPlugin
{
    private const string PluginName = nameof(FileMutatorPlugin);

    [KernelFunction("rename_file")]
    [Description("Renames a file based on its metadata and user configuration")]
    public static async Task<FileInfo> RenameFile(string filePath, string newFileName)
    {
        // Plugin implementation goes here
        var fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        fileInfo.MoveTo(newFileName);

        // Return a dummy result for now
        return await Task.FromResult(fileInfo);
    }

    [KernelFunction("move_file")]
    [Description("Moves a file to the specified destination folder")]
    public static async Task<FileInfo> MoveFile(string filePath, string destinationFolder)
    {
        var fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        var destDir = new DirectoryInfo(destinationFolder);
        if (!destDir.Exists)
        {
            throw new DirectoryNotFoundException($"Destination folder not found: {destinationFolder}");
        }

        var destFilePath = Path.Combine(destinationFolder, fileInfo.Name);
        fileInfo.MoveTo(destFilePath);

        return await Task.FromResult(fileInfo);
    }
}
