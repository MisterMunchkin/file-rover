using file_rover.dto.file;
using file_rover.dto.shared;

namespace file_rover.dto.user;

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