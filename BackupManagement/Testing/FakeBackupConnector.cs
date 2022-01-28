using FriendlyBackup.Repositories;

namespace FriendlyBackup.BackupManagement.Testing;
public class FakeBackupConnector : IBackupConnector
{
    private readonly BackupSpecs _backupSpecs;

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
        await Task.Delay(TimeSpan.FromSeconds(20));
        spec.Apply(details);
        _backupSpecs.ReplaceSpec(spec);
    }

    public Task RestoreBackupAsync(string path)
    {
        throw new NotImplementedException();
    }
}