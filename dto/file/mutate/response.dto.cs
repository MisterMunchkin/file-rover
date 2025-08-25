using System.Text.Json.Serialization;

namespace file_rover.dto.file_organise;

public class FileMutateResponseDto
{
    [JsonPropertyName("original_file")]
    public required string OriginalFile { get; set; }

    [JsonPropertyName("new_file")]
    public string? NewFile { get; set; }

    [JsonPropertyName("target_folder")]
    public string? TargetFolder { get; set; }

    [JsonPropertyName("action_log")]
    public string? ActionLog { get; set; }
}