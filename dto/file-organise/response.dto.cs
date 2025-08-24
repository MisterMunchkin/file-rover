namespace file_rover.dto.file_organise;

public class FileOrganiseResponseDto
{
    public required string OriginalFile { get; set; }
    public string? NewFile { get; set; }
    public string? TargetFolder { get; set; }
    public string? ActionLog { get; set; }

    public FileOrganiseResponseDto[] SideEffects { get; set; } = Array.Empty<FileOrganiseResponseDto>();
}