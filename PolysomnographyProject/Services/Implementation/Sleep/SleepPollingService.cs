namespace PolysomnographyProject.Services.Implementation.Sleep;

using System.Collections.Concurrent;
using Abstract.Sleep;

public class SleepPollingService : ISleepPollingService
{
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _commandQueue =
        new ConcurrentDictionary<string, TaskCompletionSource<string>>();

    public Task<string> WaitForCommandAsync(string deviceLogin)
    {
        TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>();
        _commandQueue[deviceLogin] = taskCompletionSource;
        return taskCompletionSource.Task;
    }

    public void StartSleep(string deviceLogin)
    {
        if (!_commandQueue.TryGetValue(deviceLogin, out TaskCompletionSource<string>? taskCompletionSource))
        {
            return;
        }
        
        taskCompletionSource.TrySetResult("start");
        _commandQueue.TryRemove(deviceLogin, out _);
    }

    public void StopSleep(string deviceLogin)
    {
        if (!_commandQueue.TryGetValue(deviceLogin, out TaskCompletionSource<string>? taskCompletionSource))
        {
            return;
        }
            
        taskCompletionSource.TrySetResult("stop");
        _commandQueue.TryRemove(deviceLogin, out _);
    }
}