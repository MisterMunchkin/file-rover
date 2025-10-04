using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;
using file_rover.file.renamer.bo;
using file_rover.file.renamer.metadata_extractor;
using Microsoft.SemanticKernel;

namespace file_rover.file.renamer;

public class RenameImageFileRequest
{
    [JsonPropertyName("image_path")]
    [Description("The path to the image file to be renamed.")]
    public string ImagePath { get; set; } = "";

    [JsonPropertyName("convention")]
    [Description("The naming convention using metadata fields enclosed in double curly braces, e.g., {{field_name}}-{{another_field}}")]
    public string Convention { get; set; } = "";
}

public class FileRenamerPlugin
{
    private readonly FileRenamerMetadataExtractorImageService _fileRenamerMetadataExtractorImageService;
    public FileRenamerPlugin()
    {
        _fileRenamerMetadataExtractorImageService = new FileRenamerMetadataExtractorImageService();
    }

    public static string Name { get; } = "file_renamer";

    [KernelFunction("rename_image")]
    [Description("Renames an image file based on the provided convention and file path.")]
    public async Task<string> RenameImageFile(
        [Description("The renaming request containing the convention and file path.")]
        RenameImageFileRequest request
    )
    {
        var imagePath = request.ImagePath;
        var fileInfo = new FileInfo(imagePath);
        if (!fileInfo.Exists)
            throw new FileNotFoundException($"File not found: {imagePath}");

        var fileDirectory = fileInfo.DirectoryName ?? throw new DirectoryNotFoundException("File directory not found.");

        var metadata = await _fileRenamerMetadataExtractorImageService.ExtractMetadata(imagePath);
        var convention = request.Convention;

        var imageRenamer = new FileRenamerImageBo(fileInfo, convention, metadata)
            .BuildFileName()
            .MoveToDir(fileDirectory);
        
        
        return await Task.FromResult(imageRenamer.FileName);
    }
}