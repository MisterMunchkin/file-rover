using System.Text;
using System.Text.RegularExpressions;

namespace file_rover.file.renamer.bo;

public abstract class RenamerBo<TSelf> where TSelf : RenamerBo<TSelf> {
    protected FileInfo FileInfo {get; set;}

    protected StringBuilder FileNameBuilder {get;set;}

    protected string Convention { get; set; }

    protected readonly string _conventionFieldPattern = @"\{\{(.*?)\}\}";

    public string FileName => FileNameBuilder.ToString();

    /*
    * An Enumerable of metadata fields within the convention string.
    */
    public IEnumerable<string> MetadataFields => Regex.Matches(Convention, _conventionFieldPattern)
        .Cast<Match>()
        .Select(m => m.Groups[1].Value);

    protected Dictionary<string, string> ExtractedMetadata {get; set;}

    

    public RenamerBo(FileInfo fileInfo, string convention, Dictionary<string, string>? extractedMetadata = null) {
        FileInfo = fileInfo;
        Convention = convention;
        FileNameBuilder = new(convention);
        ExtractedMetadata = extractedMetadata ?? [];
    }

    public string PrettifyFileSize(long fileSizeInBytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB", "TB"];
        double len = fileSizeInBytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }

    public void ApplyValues(string fieldMap, string? value) {
        FileNameBuilder.Replace("{{" + fieldMap + "}}", value ?? "no_value");
    }

    public virtual TSelf BuildFileName() {
        foreach (var metadataField in MetadataFields) {
            if (ExtractedMetadata.TryGetValue(metadataField, out string? metadataValue))
            {
                ApplyValues(metadataField, metadataValue);
            } else if (metadataField.Equals("file_name")) {
                ApplyValues(metadataField, Path.GetFileNameWithoutExtension(FileInfo.Name));
            } else if (metadataField.Equals("file_size")) {
                ApplyValues(metadataField, PrettifyFileSize(FileInfo.Length));
            }
        }

        return (TSelf)this;
    }

    public TSelf MoveToDir(string dir) {
        FileInfo.MoveTo(Path.Combine(dir, FileName));

        return (TSelf)this;
    }
}