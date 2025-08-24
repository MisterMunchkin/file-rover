public interface IRunner
{
    void Start();
    void Stop();
    bool IsRunning { get; }
}