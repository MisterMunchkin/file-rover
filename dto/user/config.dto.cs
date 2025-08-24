namespace file_rover.dto.user;
public class UserConfig
{
    public required FileSystemWatcherConfig FileSystemWatcher { get; set; }
}


public class FileSystemWatcherConfig
{
    public required string WatchPath { get; set; }
    public bool Enabled { get; set; }
}