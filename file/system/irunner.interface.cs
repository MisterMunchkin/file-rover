namespace file_rover.file.system;

public interface IRunner
{
    void Start();
    void Stop();
    bool IsRunning { get; }
}