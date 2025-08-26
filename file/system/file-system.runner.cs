namespace file_rover.file.system;

public class FileSystemRunner : IRunner, IDisposable
{
    private static FileSystemRunner? _instance;
    private static readonly object _lock = new();

    public static FileSystemRunner Instance(string path)
    {
        lock (_lock)
        {
            if (_instance == null || _instance._disposed)
            {
                _instance = new FileSystemRunner(path);
            }
            return _instance;
        }
    }

    public bool IsRunning { get; private set; } = false;

    public event EventHandler<string>? FileReady;

    private readonly FileSystemWatcher _watcher;
    private readonly Dictionary<string, DateTime> _pending = new();
    private readonly Timer _timer;
    private readonly TimeSpan _settleDelay = TimeSpan.FromSeconds(2);

    private bool _disposed;

    private FileSystemRunner(string path)
    {
        _watcher = new FileSystemWatcher(path)
        {
            IncludeSubdirectories = false,
            EnableRaisingEvents = false,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.Size
        };

        _watcher.Created += OnCreatedOrRenamed;
        _watcher.Renamed += OnCreatedOrRenamed;

        _timer = new Timer(CheckPending, null, 1000, 1000);

        // Dispose automatically when process exits
        AppDomain.CurrentDomain.ProcessExit += (s, e) => Dispose();
    }

    private void OnCreatedOrRenamed(object? sender, FileSystemEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Name)) return;
        if (e.Name.StartsWith('.')) return;
        if (e.Name.EndsWith(".crdownload") || e.Name.EndsWith(".tmp") || e.Name.EndsWith(".part")) return;

        lock (_pending)
        {
            _pending[e.FullPath] = DateTime.Now;
        }
    }

    private void CheckPending(object? state)
    {
        List<string> readyFiles = new();

        lock (_pending)
        {
            foreach (var kvp in _pending.ToList())
            {
                var path = kvp.Key;
                var lastSeen = kvp.Value;

                if (DateTime.Now - lastSeen >= _settleDelay)
                {
                    if (IsFileReady(path))
                    {
                        readyFiles.Add(path);
                        _pending.Remove(path);
                    }
                }
            }
        }

        foreach (var file in readyFiles)
        {
            FileReady?.Invoke(this, file);
        }
    }

    private bool IsFileReady(string path)
    {
        try
        {
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                return true;
            }
        }
        catch
        {
            return false;
        }
    }

    public void Start()
    {
        if (!IsRunning && !_disposed)
        {
            _watcher.EnableRaisingEvents = true;
            IsRunning = true;
        }
    }

    public void Stop()
    {
        if (IsRunning && !_disposed)
        {
            _watcher.EnableRaisingEvents = false;
            IsRunning = false;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        _watcher.Created -= OnCreatedOrRenamed;
        _watcher.Renamed -= OnCreatedOrRenamed;

        _watcher.Dispose();
        _timer.Dispose();

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
