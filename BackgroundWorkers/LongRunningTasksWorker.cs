using System.Collections.Concurrent;

namespace FriendlyBackup.BackgroundWorkers;

public class LongRunningTasksWorker : BackgroundService, ILongRunningRequestsRunner
{
    private readonly ConcurrentQueue<(string Id, Func<CancellationToken, Task<object?>> Function)> _longRunningTasks = new();
    private readonly ConcurrentDictionary<string, (bool Executed, object? Result)> _tasks = new(); // TODO: Clean up old tasks
    private readonly SemaphoreSlim _semaphore = new(0);

    public bool ExistsTask(string id)
        => _tasks.ContainsKey(id);

    public (bool Executed, object? Result) GetTaskResult(string id)
    {
        if (!_tasks.TryGetValue(id, out var result))
            throw new ArgumentException($"Task with id {id} does not exist", nameof(id));
        return result;
    }

    public string RunLongRunningTask<T>(Func<CancellationToken, Task<T?>> longRunningTask)
    {
        var id = Guid.NewGuid().ToString();
        _tasks.TryAdd(id, (false, default));
        _longRunningTasks.Enqueue((id, async cancellationToken => await longRunningTask(cancellationToken)));
        _semaphore.Release();
        return id;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested) 
        {
            await _semaphore.WaitAsync(stoppingToken);
            if(_longRunningTasks.TryDequeue(out var element))
            {
                var (id, function) = element;
                var res = await function(stoppingToken);
                _tasks.AddOrUpdate(id, (true, res), (id, val) => (true, res));
            }
        }
    }
}