public class FileSystemRunner : IRunner
{
    public bool IsRunning { get; private set; } = false;

    private readonly FileSystemWatcher _watcher;

    public FileSystemRunner(string path)
    {
        _watcher = new FileSystemWatcher(path);
        _watcher.Created += (s, e) => Console.WriteLine($"New file detected: {e.FullPath}");
    }

    public void Start()
    {
        if (!IsRunning)
        {
            _watcher.EnableRaisingEvents = true;
            IsRunning = true;
        }
    }

    public void Stop()
    {
        if (IsRunning)
        {
            _watcher.EnableRaisingEvents = false;
            IsRunning = false;
        }
    }
}