
using System.ComponentModel;
using System.Text.Json.Serialization;
using file_rover.user.config;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

namespace file_rover.file.mutator;

public class FileOperationResult
{
    [JsonPropertyName("file_name")]
    public string FileName { get; set; } = "";
    [JsonPropertyName("file_path")]
    public string FilePath { get; set; } = "";
    [JsonPropertyName("directory")]
    public string Directory { get; set; } = "";
}

public class FileMutatorPlugin
{
    public static string Name { get; } = "file_mutator";
    private readonly string _watchPath;

    public FileMutatorPlugin()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var userConfigService = new UserConfigService(configuration);
        _watchPath = userConfigService.WatchPath;
    }

    [KernelFunction("move_file")]
    [Description("Moves a file to the specified destination folder")]
    public async Task<FileOperationResult> MoveFile(string filePath, string destinationFolder)
    {
        var fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
            throw new FileNotFoundException($"File not found: {filePath}");
    
        var fullTargetPath = Path.GetFullPath(Path.Combine(_watchPath, destinationFolder));
        if (!fullTargetPath.StartsWith(Path.GetFullPath(_watchPath)))
            throw new InvalidOperationException("Agent tried to move file outside watch path!");
        if (!Directory.Exists(fullTargetPath))
            Directory.CreateDirectory(fullTargetPath);

        var destFilePath = Path.Combine(fullTargetPath, fileInfo.Name);
        fileInfo.MoveTo(destFilePath);

        FileOperationResult result = new()
        {
            FileName = fileInfo.Name,
            FilePath = destFilePath,
            Directory = destinationFolder
        };

        return await Task.FromResult(result);
    }
}
