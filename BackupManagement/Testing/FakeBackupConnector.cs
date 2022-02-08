using System.Collections.Concurrent;
using FriendlyBackup.Encryption;
using FriendlyBackup.Repositories;

namespace FriendlyBackup.BackupManagement.Testing;
public class FakeBackupConnector : IBackupConnector
{
    private readonly BackupSpecs _backupSpecs;
    private readonly FileEncryptor _fileEncryptor;

    private Task _sendTask;
    private readonly object _sendTaskLock = new();
    private readonly ConcurrentQueue<(string Path, IEnumerable<(byte[] EncryptedChunk, byte[] IV)> EncryptedChunks)> _sendQueue = new();

    public FakeBackupConnector(BackupSpecs backupSpecs)
    {
        _backupSpecs = backupSpecs;
    }

    public Task<(bool IsSame, BackupSpec? RemoteSpec)> CompareBackupAsync(BackupSpec spec)
    {
        return Task.FromResult((true, default(BackupSpec?)));
    }

    public async Task FixBackupAsync(BackupSpec spec)
    {
        var (isSame, remoteSpec) = await CompareBackupAsync(spec);
        if(!isSame && remoteSpec != null) 
        {
            _backupSpecs.ReplaceSpec(remoteSpec);
            var diff = remoteSpec.GetDiffBackup();
            await PerformBackupAsync(remoteSpec, diff);
        }
    }

    public async Task PerformBackupAsync(BackupSpec spec, ReadyBackupDetails details)
    {
        foreach(var elem in details.ModifiedElements) {
            var result = _fileEncryptor.EncryptFile(elem.Path);

        }
        spec.Apply(details);
        _backupSpecs.ReplaceSpec(spec);
    }

    public Task RestoreBackupAsync(string path)
    {
        throw new NotImplementedException();
    }

    public async Task SendFile(string path, IEnumerable<(byte[] EncryptedChunk, byte[] IV)> encryptedChunks)
    {
        lock(_sendTaskLock)
        {
            _sendQueue.Enqueue((path, encryptedChunks));
            if(_sendTask == null) 
            {
                _sendTask = Task.Run(async () => 
                {
                    do 
                    {
                        lock(_sendTaskLock) {
                            var success = _sendQueue.TryDequeue(out var elem);
                            if(!success)
                                break;
                        }
                        await Task.Delay(TimeSpan.FromSeconds(1)); // This will send the file chunks plus the recap for the file
                    } while(true);
                });
            }
        }
    }
    }
}