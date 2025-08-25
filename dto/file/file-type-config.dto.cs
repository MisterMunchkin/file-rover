using System.Text.Json.Serialization;

namespace file_rover.dto.file;
public class FileTypeConfig
{
    [JsonPropertyName("rename_strategy")]
    public string? RenameStrategy;

    [JsonPropertyName("metadata_fields")]
    public List<string>? MetadataFields;
}
