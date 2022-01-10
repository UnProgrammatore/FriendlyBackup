namespace FriendlyBackup.BackgroundWorkers;

public interface ILongRunningRequestsRunner : IHostedService
{
    string RunLongRunningTask<T>(Func<CancellationToken, Task<T?>> longRunningTask);
    string RunLongRunningTask(Func<CancellationToken, Task> longRunningTask)
        => RunLongRunningTask<object?>(async cancellationToken => { await longRunningTask(cancellationToken); return null; });

    (bool Executed, object? Result) GetTaskResult(string id);
    bool ExistsTask(string id);
}