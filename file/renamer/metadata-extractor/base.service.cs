namespace file_rover.file.renamer.metadata_extractor;

public abstract class FileRenamerMetadataExtractorBaseService
{
    public virtual Dictionary<string, string> SupportedMetadataFields { get; } = new()
    {
       { "file_size", "File Size" },
       { "created_at", "Created At" },
       { "modified_at", "Modified At"}
    };
    public abstract Task<Dictionary<string, object>> ExtractMetadata(string filePath);
}