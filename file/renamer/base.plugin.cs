using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;
using file_rover.file.renamer.metadata_extractor;
using Microsoft.SemanticKernel;

namespace file_rover.file.renamer;

public class RenameImageFileRequest
{
    [JsonPropertyName("convention")]
    [Description("The naming convention using metadata fields enclosed in double curly braces, e.g., {{field_name}}-{{another_field}}")]
    public string Convention { get; set; } = "";

    [JsonPropertyName("metadata_field_maps")]
    [Description("A list of metadata fields to extract and use in the naming convention.")]
    public List<string> MetadataFieldMaps { get; set; } = [];


    // convention: {{file_name}}-{{camera_model}}-{{created_at:yyyyMMdd}}
    // metadata_field_maps: ["file_name", "camera_model", "created_at"]

    //
}

public class FileRenamerPlugin
{
    private readonly FileRenamerMetadataExtractorImageService _fileRenamerMetadataExtractorImageService;
    public FileRenamerPlugin()
    {
        _fileRenamerMetadataExtractorImageService = new FileRenamerMetadataExtractorImageService();
    }

    private static readonly List<string> specialFieldMaps = [
        "file_name",
    ];

    public static string Name { get; } = "file_renamer";

    [KernelFunction("rename_image")]
    [Description("Renames an image file based on the provided convention and metadata.")]
    public async Task<string> RenameImageFile(
        [Description("The path to the image file to be renamed")]
        string imagePath,
        [Description("The renaming request containing the convention and metadata fields")]
        RenameImageFileRequest request
    )
    {
        var fileInfo = new FileInfo(imagePath);
        if (!fileInfo.Exists)
            throw new FileNotFoundException($"File not found: {imagePath}");

        var fileDirectory = fileInfo.DirectoryName ?? throw new DirectoryNotFoundException("File directory not found.");

        var metadata = await _fileRenamerMetadataExtractorImageService.ExtractMetadata(imagePath);
        var convention = request.Convention;

        StringBuilder newFileNameBuilder = new(convention);
        foreach (var fieldMap in request.MetadataFieldMaps)
        {
            //TODO: make this better
            if (specialFieldMaps.Contains(fieldMap))
            {
                if (fieldMap == "file_name")
                    ApplyMetadataValues(newFileNameBuilder, fieldMap, fileInfo.Name);
                
                continue;
            }

            if (!metadata.TryGetValue(fieldMap, out object? metaDataValue))
                throw new ArgumentException($"Metadata field '{fieldMap}' not found in extracted metadata.");

            ApplyMetadataValues(newFileNameBuilder, fieldMap, metaDataValue);
        }

        var newFileName = newFileNameBuilder.ToString();
        fileInfo.MoveTo(Path.Combine(fileDirectory, newFileName));

        return await Task.FromResult(newFileNameBuilder.ToString());
    }

    private static void ApplyMetadataValues(StringBuilder newFileNameBuilder, string fieldMap, object metaDataValue)
    {
        newFileNameBuilder.Replace("{{" + fieldMap + "}}", metaDataValue.ToString() ?? "NoValue");
    }
}