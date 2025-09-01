using file_rover.shared.dto;

namespace file_rover.user.config.dto;

public class UserConfig
{
    public  FileSystemWatcherConfig? FileSystemWatcher { get; set; }
    public  CaseType CaseType { get; set; } = CaseType.KebabCase;
    public  Dictionary<string, FileTypeConfig>? FileTypeConfig { get; set; }
}


public class FileSystemWatcherConfig
{
    public string? WatchPath { get; set; }
    public bool Enabled { get; set; } = false;
}
